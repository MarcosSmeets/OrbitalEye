namespace SatelliteTracker.Application.DTOs;

public record OrbitDto(
    Guid Id, Guid SatelliteId, double Inclination, double Eccentricity,
    double RightAscension, double ArgumentOfPerigee, double MeanAnomaly,
    double MeanMotion, DateTime Epoch, DateTime CreatedAt);
