using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Presentation.BackgroundJobs;

public class TelemetryCleanupJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelemetryCleanupJob> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _retentionPeriod = TimeSpan.FromDays(30);

    public TelemetryCleanupJob(IServiceScopeFactory scopeFactory, ILogger<TelemetryCleanupJob> logger)
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
                await CleanupTelemetryAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up telemetry data");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task CleanupTelemetryAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var telemetryRepo = scope.ServiceProvider.GetRequiredService<ITelemetryRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var cutoff = DateTime.UtcNow.Subtract(_retentionPeriod);
        var deletedCount = await telemetryRepo.DeleteOlderThanAsync(cutoff, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Telemetry cleanup completed. Deleted {Count} records older than {Cutoff:u}", deletedCount, cutoff);
    }
}
