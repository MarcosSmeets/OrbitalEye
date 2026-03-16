namespace SatelliteTracker.Domain.Entities;

public class Telemetry
{
    public Guid Id { get; private set; }
    public Guid SatelliteId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public double Altitude { get; private set; }
    public double Velocity { get; private set; }
    public double? Temperature { get; private set; }
    public double? BatteryLevel { get; private set; }
    public double? SignalStrength { get; private set; }

    private Telemetry() { }

    public static Telemetry Create(
        Guid satelliteId,
        DateTime timestamp,
        double latitude,
        double longitude,
        double altitude,
        double velocity,
        double? temperature = null,
        double? batteryLevel = null,
        double? signalStrength = null)
    {
        if (satelliteId == Guid.Empty)
            throw new ArgumentException("Satellite ID cannot be empty.", nameof(satelliteId));

        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        if (altitude < 0)
            throw new ArgumentOutOfRangeException(nameof(altitude), "Altitude must be greater than or equal to zero.");

        if (velocity < 0)
            throw new ArgumentOutOfRangeException(nameof(velocity), "Velocity must be greater than or equal to zero.");

        if (batteryLevel.HasValue && (batteryLevel.Value < 0 || batteryLevel.Value > 100))
            throw new ArgumentOutOfRangeException(nameof(batteryLevel), "Battery level must be between 0 and 100.");

        return new Telemetry
        {
            Id = Guid.NewGuid(),
            SatelliteId = satelliteId,
            Timestamp = timestamp,
            Latitude = latitude,
            Longitude = longitude,
            Altitude = altitude,
            Velocity = velocity,
            Temperature = temperature,
            BatteryLevel = batteryLevel,
            SignalStrength = signalStrength
        };
    }
}
