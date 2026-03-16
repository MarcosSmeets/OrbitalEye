using FluentAssertions;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Enums;

namespace SatelliteTracker.Domain.Tests.Entities;

public class SatelliteTests
{
    [Fact]
    public void Create_ValidParameters_ShouldCreateSatellite()
    {
        var satellite = Satellite.Create("ISS", 25544);

        satellite.Name.Should().Be("ISS");
        satellite.NoradId.Should().Be(25544);
        satellite.Id.Should().NotBeEmpty();
        satellite.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithAllOptionalParameters_ShouldSetAllProperties()
    {
        var launchDate = new DateTime(1998, 11, 20);

        var satellite = Satellite.Create(
            "ISS",
            25544,
            internationalDesignator: "1998-067A",
            @operator: "NASA",
            launchDate: launchDate);

        satellite.Name.Should().Be("ISS");
        satellite.NoradId.Should().Be(25544);
        satellite.InternationalDesignator.Should().Be("1998-067A");
        satellite.Operator.Should().Be("NASA");
        satellite.LaunchDate.Should().Be(launchDate);
    }

    [Fact]
    public void Create_DefaultStatus_ShouldBeActive()
    {
        var satellite = Satellite.Create("ISS", 25544);

        satellite.Status.Should().Be(SatelliteStatus.Active);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_EmptyOrNullName_ShouldThrowArgumentException(string? name)
    {
        Action act = () => Satellite.Create(name!, 25544);

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_InvalidNoradId_ShouldThrowArgumentException(int noradId)
    {
        Action act = () => Satellite.Create("ISS", noradId);

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("noradId");
    }

    [Fact]
    public void Deactivate_ActiveSatellite_ShouldSetStatusToInactive()
    {
        var satellite = Satellite.Create("ISS", 25544);

        satellite.Deactivate();

        satellite.Status.Should().Be(SatelliteStatus.Inactive);
    }

    [Fact]
    public void MarkDecayed_ActiveSatellite_ShouldSetStatusToDecayed()
    {
        var satellite = Satellite.Create("ISS", 25544);

        satellite.MarkDecayed();

        satellite.Status.Should().Be(SatelliteStatus.Decayed);
    }

    [Fact]
    public void Create_OptionalParametersOmitted_ShouldHaveNullDefaults()
    {
        var satellite = Satellite.Create("ISS", 25544);

        satellite.InternationalDesignator.Should().BeNull();
        satellite.Operator.Should().BeNull();
        satellite.LaunchDate.Should().BeNull();
    }
}
