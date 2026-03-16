using Microsoft.EntityFrameworkCore;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.Persistence;

namespace SatelliteTracker.Infrastructure.Repositories;

public class GroundStationRepository : IGroundStationRepository
{
    private readonly SatelliteTrackerDbContext _context;

    public GroundStationRepository(SatelliteTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<GroundStation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GroundStations
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GroundStation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.GroundStations
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GroundStation groundStation, CancellationToken cancellationToken = default)
    {
        await _context.GroundStations.AddAsync(groundStation, cancellationToken);
    }

    public void Update(GroundStation groundStation)
    {
        _context.GroundStations.Update(groundStation);
    }

    public void Delete(GroundStation groundStation)
    {
        _context.GroundStations.Remove(groundStation);
    }
}
