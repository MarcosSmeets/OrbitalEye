using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Interfaces;

public interface ITelemetryRepository
{
    Task<Telemetry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Telemetry>> GetBySatelliteIdAsync(
        Guid satelliteId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default);
    Task AddAsync(Telemetry telemetry, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Telemetry> telemetryRecords, CancellationToken cancellationToken = default);
    Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken cancellationToken = default);
}
