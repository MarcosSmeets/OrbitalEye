using Microsoft.EntityFrameworkCore;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.Persistence;

namespace SatelliteTracker.Infrastructure.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly SatelliteTrackerDbContext _context;

    public TelemetryRepository(SatelliteTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Telemetry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Telemetries
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Telemetry>> GetBySatelliteIdAsync(
        Guid satelliteId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Telemetries
            .Where(t => t.SatelliteId == satelliteId);

        if (from.HasValue)
            query = query.Where(t => t.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.Timestamp <= to.Value);

        return await query
            .OrderByDescending(t => t.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Telemetry telemetry, CancellationToken cancellationToken = default)
    {
        await _context.Telemetries.AddAsync(telemetry, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Telemetry> telemetryRecords, CancellationToken cancellationToken = default)
    {
        await _context.Telemetries.AddRangeAsync(telemetryRecords, cancellationToken);
    }

    public async Task<int> DeleteOlderThanAsync(DateTime cutoff, CancellationToken cancellationToken = default)
    {
        var oldRecords = await _context.Telemetries
            .Where(t => t.Timestamp < cutoff)
            .ToListAsync(cancellationToken);

        _context.Telemetries.RemoveRange(oldRecords);

        return oldRecords.Count;
    }
}
