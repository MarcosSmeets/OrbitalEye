using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.External;

namespace SatelliteTracker.Presentation.BackgroundJobs;

public class TleUpdateJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TleUpdateJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public TleUpdateJob(IServiceScopeFactory scopeFactory, ILogger<TleUpdateJob> logger)
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
                await UpdateTlesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating TLEs");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task UpdateTlesAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var satelliteRepo = scope.ServiceProvider.GetRequiredService<ISatelliteRepository>();
        var orbitRepo = scope.ServiceProvider.GetRequiredService<IOrbitRepository>();
        var tleProvider = scope.ServiceProvider.GetRequiredService<ITleProvider>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var satellites = await satelliteRepo.GetAllAsync(cancellationToken);

        foreach (var satellite in satellites)
        {
            try
            {
                var tleData = await tleProvider.GetTleByNoradIdAsync(satellite.NoradId, cancellationToken);
                if (tleData is null) continue;

                var lines = tleData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length < 2) continue;

                var tleLine1 = lines[0].Trim();
                var tleLine2 = lines[1].Trim();
                var parsed = TleParser.Parse(tleLine1, tleLine2);

                var orbit = SatelliteTracker.Domain.Entities.Orbit.FromTle(
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

                _logger.LogInformation("Updated TLE for satellite {Name} (NORAD {NoradId})", satellite.Name, satellite.NoradId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update TLE for satellite {NoradId}", satellite.NoradId);
            }
        }
    }
}
