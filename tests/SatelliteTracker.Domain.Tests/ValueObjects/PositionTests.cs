using FluentAssertions;
using SatelliteTracker.Domain.ValueObjects;

namespace SatelliteTracker.Domain.Tests.ValueObjects;

public class PositionTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreatePosition()
    {
        var position = new Position(45.0, 90.0, 400.0);

        position.Latitude.Should().Be(45.0);
        position.Longitude.Should().Be(90.0);
        position.Altitude.Should().Be(400.0);
    }

    [Fact]
    public void Constructor_BoundaryValues_ShouldCreatePosition()
    {
        var position = new Position(-90.0, -180.0, 0.0);

        position.Latitude.Should().Be(-90.0);
        position.Longitude.Should().Be(-180.0);
        position.Altitude.Should().Be(0.0);
    }

    [Fact]
    public void Constructor_MaxBoundaryValues_ShouldCreatePosition()
    {
        var position = new Position(90.0, 180.0, 35786.0);

        position.Latitude.Should().Be(90.0);
        position.Longitude.Should().Be(180.0);
        position.Altitude.Should().Be(35786.0);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(-200)]
    [InlineData(200)]
    public void Constructor_InvalidLatitude_ShouldThrowArgumentOutOfRangeException(double latitude)
    {
        Action act = () => new Position(latitude, 0, 400);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("latitude");
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(-360)]
    [InlineData(360)]
    public void Constructor_InvalidLongitude_ShouldThrowArgumentOutOfRangeException(double longitude)
    {
        Action act = () => new Position(0, longitude, 400);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("longitude");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_NegativeAltitude_ShouldThrowArgumentOutOfRangeException(double altitude)
    {
        Action act = () => new Position(0, 0, altitude);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .And.ParamName.Should().Be("altitude");
    }

    [Fact]
    public void Equality_SameValues_ShouldBeEqual()
    {
        var position1 = new Position(45.0, 90.0, 400.0);
        var position2 = new Position(45.0, 90.0, 400.0);

        position1.Should().Be(position2);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        var position1 = new Position(45.0, 90.0, 400.0);
        var position2 = new Position(46.0, 90.0, 400.0);

        position1.Should().NotBe(position2);
    }
}
