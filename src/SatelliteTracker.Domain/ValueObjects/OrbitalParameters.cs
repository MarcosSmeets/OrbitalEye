namespace SatelliteTracker.Domain.ValueObjects;

public record OrbitalParameters
{
    public double Inclination { get; }
    public double Eccentricity { get; }
    public double MeanMotion { get; }
    public double RightAscension { get; }
    public double ArgumentOfPerigee { get; }
    public double MeanAnomaly { get; }

    public OrbitalParameters(
        double inclination,
        double eccentricity,
        double meanMotion,
        double rightAscension,
        double argumentOfPerigee,
        double meanAnomaly)
    {
        if (inclination < 0 || inclination > 180)
            throw new ArgumentOutOfRangeException(nameof(inclination), inclination, "Inclination must be between 0 and 180 degrees.");

        if (eccentricity < 0 || eccentricity >= 1)
            throw new ArgumentOutOfRangeException(nameof(eccentricity), eccentricity, "Eccentricity must be between 0 (inclusive) and 1 (exclusive).");

        if (meanMotion <= 0)
            throw new ArgumentOutOfRangeException(nameof(meanMotion), meanMotion, "Mean motion must be greater than 0.");

        if (rightAscension < 0 || rightAscension > 360)
            throw new ArgumentOutOfRangeException(nameof(rightAscension), rightAscension, "Right ascension must be between 0 and 360 degrees.");

        if (argumentOfPerigee < 0 || argumentOfPerigee > 360)
            throw new ArgumentOutOfRangeException(nameof(argumentOfPerigee), argumentOfPerigee, "Argument of perigee must be between 0 and 360 degrees.");

        if (meanAnomaly < 0 || meanAnomaly > 360)
            throw new ArgumentOutOfRangeException(nameof(meanAnomaly), meanAnomaly, "Mean anomaly must be between 0 and 360 degrees.");

        Inclination = inclination;
        Eccentricity = eccentricity;
        MeanMotion = meanMotion;
        RightAscension = rightAscension;
        ArgumentOfPerigee = argumentOfPerigee;
        MeanAnomaly = meanAnomaly;
    }
}
