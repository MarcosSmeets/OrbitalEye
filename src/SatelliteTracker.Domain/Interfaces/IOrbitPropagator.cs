namespace SatelliteTracker.Domain.Interfaces;

public interface IOrbitPropagator
{
    (double Latitude, double Longitude, double Altitude) CalculatePosition(Entities.Orbit orbit, DateTime atTime);
    (double Latitude, double Longitude, double Altitude, double Velocity) CalculatePositionFromTle(string tleLine1, string tleLine2, DateTime atTime);
}
