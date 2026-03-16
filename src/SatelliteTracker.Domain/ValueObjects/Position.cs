namespace SatelliteTracker.Domain.ValueObjects;

public record Position
{
    public double Latitude { get; }
    public double Longitude { get; }
    public double Altitude { get; }

    public Position(double latitude, double longitude, double altitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), latitude, "Latitude must be between -90 and 90 degrees.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), longitude, "Longitude must be between -180 and 180 degrees.");

        if (altitude < 0)
            throw new ArgumentOutOfRangeException(nameof(altitude), altitude, "Altitude must be greater than or equal to 0.");

        Latitude = latitude;
        Longitude = longitude;
        Altitude = altitude;
    }
}
