using FluentAssertions;
using SatelliteTracker.Domain.ValueObjects;

namespace SatelliteTracker.Domain.Tests.ValueObjects;

public class OrbitalParametersTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateOrbitalParameters()
    {
        var parameters = new OrbitalParameters(51.6, 0.0001, 15.5, 200.0, 100.0, 50.0);

        parameters.Inclination.Should().Be(51.6);
        parameters.Eccentricity.Should().Be(0.0001);
        parameters.MeanMotion.Should().Be(15.5);
        parameters.RightAscension.Should().Be(200.0);
        parameters.ArgumentOfPerigee.Should().Be(100.0);
        parameters.MeanAnomaly.Should().Be(50.0);
    }

    [Fact]
    public void Constructor_BoundaryValues_ShouldCreateOrbitalParameters()
    {
        var parameters = new OrbitalParameters(0, 0, 0.001, 0, 0, 0);

        parameters.Inclination.Should().Be(0);
        parameters.Eccentricity.Should().Be(0);
        parameters.MeanMotion.Should().Be(0.001);
        parameters.RightAscension.Should().Be(0);
        parameters.ArgumentOfPerigee.Should().Be(0);
        parameters.MeanAnomaly.Should().Be(0);
    }

    [Fact]
    public void Constructor_MaxBoundaryValues_ShouldCreateOrbitalParameters()
    {
        var parameters = new OrbitalParameters(180, 0.999, 17.0, 360, 360, 360);

        parameters.Inclination.Should().Be(180);
        parameters.Eccentricity.Should().Be(0.999);
        parameters.RightAscension.Should().Be(360);
        parameters.ArgumentOfPerigee.Should().Be(360);
        parameters.MeanAnomaly.Should().Be(360);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(181)]
    [InlineData(-0.1)]
    public void Constructor_InvalidInclination_ShouldThrowArgumentOutOfRangeException(double inclination)
    {
        Action act = () => new OrbitalParameters(inclination, 0.5, 15.0, 200, 100, 50);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("inclination");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    public void Constructor_InvalidEccentricity_ShouldThrowArgumentOutOfRangeException(double eccentricity)
    {
        Action act = () => new OrbitalParameters(51.6, eccentricity, 15.0, 200, 100, 50);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("eccentricity");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-15.5)]
    public void Constructor_InvalidMeanMotion_ShouldThrowArgumentOutOfRangeException(double meanMotion)
    {
        Action act = () => new OrbitalParameters(51.6, 0.5, meanMotion, 200, 100, 50);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("meanMotion");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(361)]
    public void Constructor_InvalidRightAscension_ShouldThrowArgumentOutOfRangeException(double rightAscension)
    {
        Action act = () => new OrbitalParameters(51.6, 0.5, 15.0, rightAscension, 100, 50);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("rightAscension");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(361)]
    public void Constructor_InvalidArgumentOfPerigee_ShouldThrowArgumentOutOfRangeException(double argumentOfPerigee)
    {
        Action act = () => new OrbitalParameters(51.6, 0.5, 15.0, 200, argumentOfPerigee, 50);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("argumentOfPerigee");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(361)]
    public void Constructor_InvalidMeanAnomaly_ShouldThrowArgumentOutOfRangeException(double meanAnomaly)
    {
        Action act = () => new OrbitalParameters(51.6, 0.5, 15.0, 200, 100, meanAnomaly);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("meanAnomaly");
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        var params1 = new OrbitalParameters(51.6, 0.0001, 15.5, 200.0, 100.0, 50.0);
        var params2 = new OrbitalParameters(51.6, 0.0001, 15.5, 200.0, 100.0, 50.0);

        params1.Should().Be(params2);
    }
}
