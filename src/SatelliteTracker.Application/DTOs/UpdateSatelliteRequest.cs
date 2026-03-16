namespace SatelliteTracker.Application.DTOs;

public record UpdateSatelliteRequest(
    string? Name = null, string? Operator = null, string? Status = null);
