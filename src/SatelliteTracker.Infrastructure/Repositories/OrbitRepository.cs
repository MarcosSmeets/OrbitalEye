using Microsoft.EntityFrameworkCore;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.Persistence;

namespace SatelliteTracker.Infrastructure.Repositories;

public class OrbitRepository : IOrbitRepository
{
    private readonly SatelliteTrackerDbContext _context;

    public OrbitRepository(SatelliteTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Orbit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orbits
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Orbit?> GetBySatelliteIdAsync(Guid satelliteId, CancellationToken cancellationToken = default)
    {
        return await _context.Orbits
            .Where(o => o.SatelliteId == satelliteId)
            .OrderByDescending(o => o.Epoch)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Orbit>> GetAllBySatelliteIdAsync(Guid satelliteId, CancellationToken cancellationToken = default)
    {
        return await _context.Orbits
            .Where(o => o.SatelliteId == satelliteId)
            .OrderByDescending(o => o.Epoch)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Orbit orbit, CancellationToken cancellationToken = default)
    {
        await _context.Orbits.AddAsync(orbit, cancellationToken);
    }
}
