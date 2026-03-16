using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Infrastructure.External;

public class CelestrakClient : ITleProvider
{
    private readonly HttpClient _httpClient;

    public CelestrakClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> GetTleByNoradIdAsync(int noradId, CancellationToken cancellationToken = default)
    {
        var url = $"https://celestrak.org/NORAD/elements/gp.php?CATNR={noradId}&FORMAT=TLE";

        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(content))
            return null;

        return content.Trim();
    }

    public async Task<IReadOnlyList<(int NoradId, string TleLine1, string TleLine2)>> GetActiveSatelliteTlesAsync(CancellationToken cancellationToken = default)
    {
        var url = "https://celestrak.org/NORAD/elements/gp.php?GROUP=active&FORMAT=TLE";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var results = new List<(int NoradId, string TleLine1, string TleLine2)>();

        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        for (var i = 0; i + 2 < lines.Length; i += 3)
        {
            var line1 = lines[i + 1];
            var line2 = lines[i + 2];

            if (line1.Length < 9 || !line1.StartsWith("1 "))
                continue;

            if (line2.Length < 9 || !line2.StartsWith("2 "))
                continue;

            if (int.TryParse(line2.Substring(2, 5).Trim(), out var noradId))
            {
                results.Add((noradId, line1, line2));
            }
        }

        return results;
    }
}
