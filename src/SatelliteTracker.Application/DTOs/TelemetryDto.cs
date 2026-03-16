namespace SatelliteTracker.Application.DTOs;

public record TelemetryDto(
    Guid Id, Guid SatelliteId, DateTime Timestamp,
    double Latitude, double Longitude, double Altitude, double Velocity,
    double? Temperature, double? BatteryLevel, double? SignalStrength);
