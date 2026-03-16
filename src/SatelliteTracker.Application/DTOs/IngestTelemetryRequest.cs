namespace SatelliteTracker.Application.DTOs;

public record IngestTelemetryRequest(
    Guid SatelliteId, DateTime Timestamp,
    double Latitude, double Longitude, double Altitude, double Velocity,
    double? Temperature = null, double? BatteryLevel = null, double? SignalStrength = null);
