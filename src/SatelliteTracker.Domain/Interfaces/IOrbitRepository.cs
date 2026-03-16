using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Interfaces;

public interface IOrbitRepository
{
    Task<Orbit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Orbit?> GetBySatelliteIdAsync(Guid satelliteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Orbit>> GetAllBySatelliteIdAsync(Guid satelliteId, CancellationToken cancellationToken = default);
    Task AddAsync(Orbit orbit, CancellationToken cancellationToken = default);
}
