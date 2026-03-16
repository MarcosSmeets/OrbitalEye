using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Infrastructure.External;

public class SpaceTrackClient : ITleProvider
{
    private const string BaseUrl = "https://www.space-track.org";
    private const string LoginUrl = $"{BaseUrl}/ajaxauth/login";
    private const string QueryUrl = $"{BaseUrl}/basicspacedata/query";

    private readonly HttpClient _httpClient;
    private readonly string _identity;
    private readonly string _password;
    private readonly ILogger<SpaceTrackClient> _logger;
    private bool _isAuthenticated;
    private readonly SemaphoreSlim _authLock = new(1, 1);

    public SpaceTrackClient(HttpClient httpClient, IConfiguration configuration, ILogger<SpaceTrackClient> logger)
    {
        _httpClient = httpClient;
        _identity = configuration["SpaceTrack:Identity"] ?? "";
        _password = configuration["SpaceTrack:Password"] ?? "";
        _logger = logger;
    }

    private async Task EnsureAuthenticatedAsync(CancellationToken cancellationToken)
    {
        if (_isAuthenticated) return;

        await _authLock.WaitAsync(cancellationToken);
        try
        {
            if (_isAuthenticated) return;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("identity", _identity),
                new KeyValuePair<string, string>("password", _password)
            });

            var response = await _httpClient.PostAsync(LoginUrl, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            _isAuthenticated = true;
            _logger.LogInformation("Successfully authenticated with Space-Track.org");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to authenticate with Space-Track.org");
            throw;
        }
        finally
        {
            _authLock.Release();
        }
    }

    public async Task<string?> GetTleByNoradIdAsync(int noradId, CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var url = $"{QueryUrl}/class/gp/NORAD_CAT_ID/{noradId}/orderby/EPOCH desc/limit/1/format/tle";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return string.IsNullOrWhiteSpace(content) ? null : content.Trim();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to fetch TLE for NORAD ID {NoradId} from Space-Track", noradId);
            // Reset auth on 401
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
                _isAuthenticated = false;
            return null;
        }
    }

    public async Task<IReadOnlyList<(int NoradId, string TleLine1, string TleLine2)>> GetActiveSatelliteTlesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureAuthenticatedAsync(cancellationToken);

        var url = $"{QueryUrl}/class/gp/DECAY_DATE/null-val/EPOCH/%3Enow-30/orderby/NORAD_CAT_ID/format/tle";

        try
        {
            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return ParseTleList(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to fetch active satellite TLEs from Space-Track");
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
                _isAuthenticated = false;
            return Array.Empty<(int, string, string)>();
        }
    }

    private static IReadOnlyList<(int NoradId, string TleLine1, string TleLine2)> ParseTleList(string content)
    {
        var results = new List<(int, string, string)>();
        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        for (var i = 0; i + 1 < lines.Length; i += 2)
        {
            var line1 = lines[i];
            var line2 = lines[i + 1];

            if (line1.StartsWith("1 ") && line2.StartsWith("2 "))
            {
                if (int.TryParse(line1.Substring(2, 5).Trim(), out var noradId))
                {
                    results.Add((noradId, line1, line2));
                }
            }
        }

        return results;
    }
}
