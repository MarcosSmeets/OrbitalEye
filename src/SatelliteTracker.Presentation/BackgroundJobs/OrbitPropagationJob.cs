using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SatelliteTracker.Application.Interfaces;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Presentation.BackgroundJobs;

public class OrbitPropagationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOrbitPropagator _propagator;
    private readonly ITelemetryBroadcaster _broadcaster;
    private readonly ILogger<OrbitPropagationJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    public OrbitPropagationJob(
        IServiceScopeFactory scopeFactory,
        IOrbitPropagator propagator,
        ITelemetryBroadcaster broadcaster,
        ILogger<OrbitPropagationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _propagator = propagator;
        _broadcaster = broadcaster;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait a bit for TLE data to be fetched first
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PropagateOrbitsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error propagating orbits");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task PropagateOrbitsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var satelliteRepo = scope.ServiceProvider.GetRequiredService<ISatelliteRepository>();
        var orbitRepo = scope.ServiceProvider.GetRequiredService<IOrbitRepository>();

        var satellites = await satelliteRepo.GetAllAsync(cancellationToken);
        var now = DateTime.UtcNow;

        foreach (var satellite in satellites)
        {
            try
            {
                var orbit = await orbitRepo.GetBySatelliteIdAsync(satellite.Id, cancellationToken);
                if (orbit is null) continue;

                var tleLine1 = orbit.TleLine1;
                var tleLine2 = orbit.TleLine2;

                if (string.IsNullOrEmpty(tleLine1) || string.IsNullOrEmpty(tleLine2))
                    continue;

                var (lat, lon, alt, _) = _propagator.CalculatePositionFromTle(tleLine1, tleLine2, now);

                await _broadcaster.BroadcastPositionUpdateAsync(
                    satellite.Id, lat, lon, alt, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to propagate orbit for satellite {Name} ({NoradId})",
                    satellite.Name, satellite.NoradId);
            }
        }
    }
}
