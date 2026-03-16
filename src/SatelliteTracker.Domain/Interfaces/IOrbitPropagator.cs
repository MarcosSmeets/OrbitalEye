namespace SatelliteTracker.Domain.Interfaces;

public interface IOrbitPropagator
{
    (double Latitude, double Longitude, double Altitude) CalculatePosition(Entities.Orbit orbit, DateTime atTime);
}
