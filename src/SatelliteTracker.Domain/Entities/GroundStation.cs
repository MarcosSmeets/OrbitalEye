namespace SatelliteTracker.Domain.Entities;

public class GroundStation
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double Altitude { get; private set; }
    public string Country { get; private set; }

    private GroundStation()
    {
        Name = null!;
        Country = null!;
    }

    public static GroundStation Create(
        string name,
        double latitude,
        double longitude,
        double altitude,
        string country)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty.", nameof(country));

        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        if (altitude < 0)
            throw new ArgumentOutOfRangeException(nameof(altitude), "Altitude must be greater than or equal to zero.");

        return new GroundStation
        {
            Id = Guid.NewGuid(),
            Name = name,
            Latitude = latitude,
            Longitude = longitude,
            Altitude = altitude,
            Country = country
        };
    }
}
