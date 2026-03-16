namespace SatelliteTracker.Application.DTOs;

public record CreateSatelliteRequest(
    string Name, int NoradId, string? InternationalDesignator = null,
    string? Operator = null, DateTime? LaunchDate = null);
