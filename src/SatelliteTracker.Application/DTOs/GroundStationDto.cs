namespace SatelliteTracker.Application.DTOs;

public record GroundStationDto(
    Guid Id, string Name, double Latitude, double Longitude, double Altitude, string Country);
