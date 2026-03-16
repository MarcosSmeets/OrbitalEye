namespace SatelliteTracker.Domain.Entities;

public class Orbit
{
    public Guid Id { get; private set; }
    public Guid SatelliteId { get; private set; }
    public double Inclination { get; private set; }
    public double Eccentricity { get; private set; }
    public double RightAscension { get; private set; }
    public double ArgumentOfPerigee { get; private set; }
    public double MeanAnomaly { get; private set; }
    public double MeanMotion { get; private set; }
    public DateTime Epoch { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Orbit() { }

    public static Orbit Create(
        Guid satelliteId,
        double inclination,
        double eccentricity,
        double rightAscension,
        double argumentOfPerigee,
        double meanAnomaly,
        double meanMotion,
        DateTime epoch)
    {
        if (satelliteId == Guid.Empty)
            throw new ArgumentException("Satellite ID cannot be empty.", nameof(satelliteId));

        if (inclination < 0 || inclination > 180)
            throw new ArgumentOutOfRangeException(nameof(inclination), "Inclination must be between 0 and 180 degrees.");

        if (eccentricity < 0 || eccentricity >= 1)
            throw new ArgumentOutOfRangeException(nameof(eccentricity), "Eccentricity must be between 0 (inclusive) and 1 (exclusive).");

        if (meanMotion <= 0)
            throw new ArgumentOutOfRangeException(nameof(meanMotion), "Mean motion must be greater than zero.");

        return new Orbit
        {
            Id = Guid.NewGuid(),
            SatelliteId = satelliteId,
            Inclination = inclination,
            Eccentricity = eccentricity,
            RightAscension = rightAscension,
            ArgumentOfPerigee = argumentOfPerigee,
            MeanAnomaly = meanAnomaly,
            MeanMotion = meanMotion,
            Epoch = epoch,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Orbit FromTle(
        Guid satelliteId,
        double inclination,
        double eccentricity,
        double rightAscension,
        double argumentOfPerigee,
        double meanAnomaly,
        double meanMotion,
        DateTime epoch)
    {
        return Create(
            satelliteId,
            inclination,
            eccentricity,
            rightAscension,
            argumentOfPerigee,
            meanAnomaly,
            meanMotion,
            epoch);
    }
}
