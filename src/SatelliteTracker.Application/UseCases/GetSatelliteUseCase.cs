using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class GetSatelliteUseCase
{
    private readonly ISatelliteRepository _repository;

    public GetSatelliteUseCase(ISatelliteRepository repository)
    {
        _repository = repository;
    }

    public async Task<SatelliteDto?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var satellite = await _repository.GetByIdAsync(id, cancellationToken);
        return satellite is null ? null : SatelliteMapper.ToDto(satellite);
    }
}
