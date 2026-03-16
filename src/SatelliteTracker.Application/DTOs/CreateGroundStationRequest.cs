namespace SatelliteTracker.Application.DTOs;

public record CreateGroundStationRequest(
    string Name, double Latitude, double Longitude, double Altitude, string Country);
