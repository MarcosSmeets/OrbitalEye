namespace SatelliteTracker.Application.DTOs;

public record SatelliteDto(
    Guid Id, string Name, int NoradId, string? InternationalDesignator,
    DateTime? LaunchDate, string? Operator, string Status, DateTime CreatedAt);
