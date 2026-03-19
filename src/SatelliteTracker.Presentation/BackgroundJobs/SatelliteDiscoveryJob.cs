using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.External;

namespace SatelliteTracker.Presentation.BackgroundJobs;

public class SatelliteDiscoveryJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SatelliteDiscoveryJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public SatelliteDiscoveryJob(IServiceScopeFactory scopeFactory, ILogger<SatelliteDiscoveryJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DiscoverSatellitesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during satellite discovery");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task DiscoverSatellitesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var satelliteRepo = scope.ServiceProvider.GetRequiredService<ISatelliteRepository>();
        var orbitRepo = scope.ServiceProvider.GetRequiredService<IOrbitRepository>();
        var tleProvider = scope.ServiceProvider.GetRequiredService<ITleProvider>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var activeTles = await tleProvider.GetActiveSatelliteTlesAsync(cancellationToken);
        var discoveredCount = 0;

        foreach (var (name, noradId, tleLine1, tleLine2) in activeTles)
        {
            try
            {
                var existing = await satelliteRepo.GetByNoradIdAsync(noradId, cancellationToken);
                if (existing is not null)
                    continue;

                var satellite = Domain.Entities.Satellite.Create(name, noradId);
                await satelliteRepo.AddAsync(satellite, cancellationToken);

                var parsed = TleParser.Parse(tleLine1, tleLine2);
                var orbit = Domain.Entities.Orbit.FromTle(
                    satellite.Id,
                    parsed.Inclination,
                    parsed.Eccentricity,
                    parsed.RightAscension,
                    parsed.ArgumentOfPerigee,
                    parsed.MeanAnomaly,
                    parsed.MeanMotion,
                    parsed.Epoch,
                    tleLine1,
                    tleLine2);

                await orbitRepo.AddAsync(orbit, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                discoveredCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to discover satellite with NORAD ID {NoradId}", noradId);
            }
        }

        _logger.LogInformation("Satellite discovery completed. Discovered {Count} new satellites", discoveredCount);
    }
}
