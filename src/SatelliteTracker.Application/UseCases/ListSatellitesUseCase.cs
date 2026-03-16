using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class ListSatellitesUseCase
{
    private readonly ISatelliteRepository _repository;

    public ListSatellitesUseCase(ISatelliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<SatelliteDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var satellites = await _repository.GetAllAsync(cancellationToken);
        return satellites.Select(SatelliteMapper.ToDto).ToList();
    }
}
