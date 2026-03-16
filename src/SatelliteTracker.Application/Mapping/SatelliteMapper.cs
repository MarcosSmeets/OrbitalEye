using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Application.Mapping;

public static class SatelliteMapper
{
    public static SatelliteDto ToDto(Satellite satellite) =>
        new(
            satellite.Id,
            satellite.Name,
            satellite.NoradId,
            satellite.InternationalDesignator,
            satellite.LaunchDate,
            satellite.Operator,
            satellite.Status.ToString(),
            satellite.CreatedAt);

    public static OrbitDto ToDto(Orbit orbit) =>
        new(
            orbit.Id,
            orbit.SatelliteId,
            orbit.Inclination,
            orbit.Eccentricity,
            orbit.RightAscension,
            orbit.ArgumentOfPerigee,
            orbit.MeanAnomaly,
            orbit.MeanMotion,
            orbit.Epoch,
            orbit.CreatedAt);

    public static TelemetryDto ToDto(Telemetry telemetry) =>
        new(
            telemetry.Id,
            telemetry.SatelliteId,
            telemetry.Timestamp,
            telemetry.Latitude,
            telemetry.Longitude,
            telemetry.Altitude,
            telemetry.Velocity,
            telemetry.Temperature,
            telemetry.BatteryLevel,
            telemetry.SignalStrength);

    public static GroundStationDto ToDto(GroundStation station) =>
        new(
            station.Id,
            station.Name,
            station.Latitude,
            station.Longitude,
            station.Altitude,
            station.Country);
}
