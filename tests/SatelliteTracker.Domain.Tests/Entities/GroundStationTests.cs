using FluentAssertions;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Tests.Entities;

public class GroundStationTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateGroundStation()
    {
        var station = GroundStation.Create("Goldstone", 35.4267, -116.89, 900.0, "USA");

        station.Id.Should().NotBeEmpty();
        station.Name.Should().Be("Goldstone");
        station.Latitude.Should().Be(35.4267);
        station.Longitude.Should().Be(-116.89);
        station.Altitude.Should().Be(900.0);
        station.Country.Should().Be("USA");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyOrNullName_ShouldThrowArgumentException(string? name)
    {
        Action act = () => GroundStation.Create(name!, 35.0, -116.0, 900.0, "USA");

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyOrNullCountry_ShouldThrowArgumentException(string? country)
    {
        Action act = () => GroundStation.Create("Goldstone", 35.0, -116.0, 900.0, country!);

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("country");
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(-200)]
    public void Create_InvalidLatitude_ShouldThrowArgumentOutOfRangeException(double latitude)
    {
        Action act = () => GroundStation.Create("Goldstone", latitude, -116.0, 900.0, "USA");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(360)]
    public void Create_InvalidLongitude_ShouldThrowArgumentOutOfRangeException(double longitude)
    {
        Action act = () => GroundStation.Create("Goldstone", 35.0, longitude, 900.0, "USA");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_NegativeAltitude_ShouldThrowArgumentOutOfRangeException(double altitude)
    {
        Action act = () => GroundStation.Create("Goldstone", 35.0, -116.0, altitude, "USA");

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_BoundaryCoordinates_ShouldCreateGroundStation()
    {
        var station = GroundStation.Create("South Pole Station", -90.0, -180.0, 0.0, "Antarctica");

        station.Latitude.Should().Be(-90.0);
        station.Longitude.Should().Be(-180.0);
        station.Altitude.Should().Be(0.0);
    }
}
