using FluentAssertions;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Tests.Entities;

public class OrbitTests
{
    private readonly Guid _satelliteId = Guid.NewGuid();
    private readonly DateTime _epoch = DateTime.UtcNow;

    [Fact]
    public void Create_ValidParameters_ShouldCreateOrbit()
    {
        var orbit = Orbit.Create(_satelliteId, 51.6, 0.0001, 200.0, 100.0, 50.0, 15.5, _epoch);

        orbit.Id.Should().NotBeEmpty();
        orbit.SatelliteId.Should().Be(_satelliteId);
        orbit.Inclination.Should().Be(51.6);
        orbit.Eccentricity.Should().Be(0.0001);
        orbit.RightAscension.Should().Be(200.0);
        orbit.ArgumentOfPerigee.Should().Be(100.0);
        orbit.MeanAnomaly.Should().Be(50.0);
        orbit.MeanMotion.Should().Be(15.5);
        orbit.Epoch.Should().Be(_epoch);
        orbit.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_EmptySatelliteId_ShouldThrowArgumentException()
    {
        Action act = () => Orbit.Create(Guid.Empty, 51.6, 0.0001, 200.0, 100.0, 50.0, 15.5, _epoch);

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("satelliteId");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(181)]
    [InlineData(-0.1)]
    public void Create_InvalidInclination_ShouldThrowArgumentOutOfRangeException(double inclination)
    {
        Action act = () => Orbit.Create(_satelliteId, inclination, 0.0001, 200.0, 100.0, 50.0, 15.5, _epoch);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    public void Create_InvalidEccentricity_ShouldThrowArgumentOutOfRangeException(double eccentricity)
    {
        Action act = () => Orbit.Create(_satelliteId, 51.6, eccentricity, 200.0, 100.0, 50.0, 15.5, _epoch);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-15.5)]
    public void Create_InvalidMeanMotion_ShouldThrowArgumentOutOfRangeException(double meanMotion)
    {
        Action act = () => Orbit.Create(_satelliteId, 51.6, 0.0001, 200.0, 100.0, 50.0, meanMotion, _epoch);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void FromTle_ValidParameters_ShouldCreateOrbitSameAsCreate()
    {
        var orbitFromTle = Orbit.FromTle(_satelliteId, 51.6, 0.0001, 200.0, 100.0, 50.0, 15.5, _epoch);

        orbitFromTle.Id.Should().NotBeEmpty();
        orbitFromTle.SatelliteId.Should().Be(_satelliteId);
        orbitFromTle.Inclination.Should().Be(51.6);
        orbitFromTle.Eccentricity.Should().Be(0.0001);
        orbitFromTle.RightAscension.Should().Be(200.0);
        orbitFromTle.ArgumentOfPerigee.Should().Be(100.0);
        orbitFromTle.MeanAnomaly.Should().Be(50.0);
        orbitFromTle.MeanMotion.Should().Be(15.5);
        orbitFromTle.Epoch.Should().Be(_epoch);
    }

    [Fact]
    public void FromTle_InvalidParameters_ShouldThrowSameAsCreate()
    {
        Action act = () => Orbit.FromTle(Guid.Empty, 51.6, 0.0001, 200.0, 100.0, 50.0, 15.5, _epoch);

        act.Should().Throw<ArgumentException>();
    }
}
