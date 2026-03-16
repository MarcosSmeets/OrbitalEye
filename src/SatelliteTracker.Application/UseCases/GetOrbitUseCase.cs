using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class GetOrbitUseCase
{
    private readonly IOrbitRepository _repository;

    public GetOrbitUseCase(IOrbitRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrbitDto?> ExecuteAsync(Guid satelliteId, CancellationToken cancellationToken = default)
    {
        var orbit = await _repository.GetBySatelliteIdAsync(satelliteId, cancellationToken);
        return orbit is null ? null : SatelliteMapper.ToDto(orbit);
    }
}
