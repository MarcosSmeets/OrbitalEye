using SatelliteTracker.Application.DTOs;

namespace SatelliteTracker.Application.Interfaces;

public interface ITelemetryBroadcaster
{
    Task BroadcastPositionUpdateAsync(Guid satelliteId, double latitude, double longitude, double altitude, CancellationToken cancellationToken = default);
    Task BroadcastTelemetryUpdateAsync(Guid satelliteId, TelemetryDto telemetry, CancellationToken cancellationToken = default);
}
