namespace SatelliteTracker.Domain.ValueObjects;

public record Velocity
{
    public double Magnitude { get; }
    public double Direction { get; }

    public Velocity(double magnitude, double direction)
    {
        if (magnitude < 0)
            throw new ArgumentOutOfRangeException(nameof(magnitude), magnitude, "Magnitude must be greater than or equal to 0.");

        Magnitude = magnitude;
        Direction = direction;
    }
}
