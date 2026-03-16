using Microsoft.EntityFrameworkCore;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;
using SatelliteTracker.Infrastructure.Persistence;

namespace SatelliteTracker.Infrastructure.Repositories;

public class SatelliteRepository : ISatelliteRepository
{
    private readonly SatelliteTrackerDbContext _context;

    public SatelliteRepository(SatelliteTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<Satellite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Satellites
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Satellite?> GetByNoradIdAsync(int noradId, CancellationToken cancellationToken = default)
    {
        return await _context.Satellites
            .FirstOrDefaultAsync(s => s.NoradId == noradId, cancellationToken);
    }

    public async Task<IReadOnlyList<Satellite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Satellites
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Satellite satellite, CancellationToken cancellationToken = default)
    {
        await _context.Satellites.AddAsync(satellite, cancellationToken);
    }

    public void Update(Satellite satellite)
    {
        _context.Satellites.Update(satellite);
    }

    public void Delete(Satellite satellite)
    {
        _context.Satellites.Remove(satellite);
    }
}
