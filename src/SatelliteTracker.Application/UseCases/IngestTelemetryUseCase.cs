using FluentValidation;
using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Interfaces;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Application.Validators;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Application.UseCases;

public class IngestTelemetryUseCase
{
    private readonly ITelemetryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITelemetryBroadcaster? _broadcaster;

    public IngestTelemetryUseCase(
        ITelemetryRepository repository,
        IUnitOfWork unitOfWork,
        ITelemetryBroadcaster? broadcaster = null)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _broadcaster = broadcaster;
    }

    public async Task<TelemetryDto> ExecuteAsync(IngestTelemetryRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new IngestTelemetryRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var telemetry = Telemetry.Create(
            request.SatelliteId,
            request.Timestamp,
            request.Latitude,
            request.Longitude,
            request.Altitude,
            request.Velocity,
            request.Temperature,
            request.BatteryLevel,
            request.SignalStrength);

        await _repository.AddAsync(telemetry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = SatelliteMapper.ToDto(telemetry);

        if (_broadcaster is not null)
        {
            await _broadcaster.BroadcastPositionUpdateAsync(
                request.SatelliteId, request.Latitude, request.Longitude, request.Altitude, cancellationToken);
            await _broadcaster.BroadcastTelemetryUpdateAsync(
                request.SatelliteId, dto, cancellationToken);
        }

        return dto;
    }
}
