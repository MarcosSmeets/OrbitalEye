using SatelliteTracker.Domain.Enums;

namespace SatelliteTracker.Domain.Entities;

public class Satellite
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public int NoradId { get; private set; }
    public string? InternationalDesignator { get; private set; }
    public DateTime? LaunchDate { get; private set; }
    public string? Operator { get; private set; }
    public SatelliteStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Satellite()
    {
        Name = null!;
    }

    public static Satellite Create(
        string name,
        int noradId,
        string? internationalDesignator = null,
        string? @operator = null,
        DateTime? launchDate = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (noradId <= 0)
            throw new ArgumentException("NORAD ID must be greater than zero.", nameof(noradId));

        return new Satellite
        {
            Id = Guid.NewGuid(),
            Name = name,
            NoradId = noradId,
            InternationalDesignator = internationalDesignator,
            Operator = @operator,
            LaunchDate = launchDate,
            Status = SatelliteStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        Status = SatelliteStatus.Inactive;
    }

    public void MarkDecayed()
    {
        Status = SatelliteStatus.Decayed;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name;
    }

    public void UpdateOperator(string? @operator)
    {
        Operator = @operator;
    }

    public void UpdateStatus(SatelliteStatus status)
    {
        Status = status;
    }
}
