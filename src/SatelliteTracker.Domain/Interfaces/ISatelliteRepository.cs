using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Interfaces;

public interface ISatelliteRepository
{
    Task<Satellite?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Satellite?> GetByNoradIdAsync(int noradId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Satellite>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Satellite satellite, CancellationToken cancellationToken = default);
    void Update(Satellite satellite);
    void Delete(Satellite satellite);
}
