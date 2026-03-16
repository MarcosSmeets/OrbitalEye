using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class GetTelemetryHistoryUseCase
{
    private readonly ITelemetryRepository _repository;

    public GetTelemetryHistoryUseCase(ITelemetryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TelemetryDto>> ExecuteAsync(
        Guid satelliteId,
        DateTime? from = null,
        DateTime? to = null,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var telemetryRecords = await _repository.GetBySatelliteIdAsync(satelliteId, from, to, limit, cancellationToken);
        return telemetryRecords.Select(SatelliteMapper.ToDto).ToList();
    }
}
