using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Interfaces;

public interface IGroundStationRepository
{
    Task<GroundStation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroundStation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(GroundStation groundStation, CancellationToken cancellationToken = default);
    void Update(GroundStation groundStation);
    void Delete(GroundStation groundStation);
}
