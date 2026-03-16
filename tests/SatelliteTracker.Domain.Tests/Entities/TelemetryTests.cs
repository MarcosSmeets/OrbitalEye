using FluentAssertions;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Domain.Tests.Entities;

public class TelemetryTests
{
    private readonly Guid _satelliteId = Guid.NewGuid();
    private readonly DateTime _timestamp = DateTime.UtcNow;

    [Fact]
    public void Create_ValidParameters_ShouldCreateTelemetry()
    {
        var telemetry = Telemetry.Create(_satelliteId, _timestamp, 45.0, 90.0, 408.0, 7.66);

        telemetry.Id.Should().NotBeEmpty();
        telemetry.SatelliteId.Should().Be(_satelliteId);
        telemetry.Timestamp.Should().Be(_timestamp);
        telemetry.Latitude.Should().Be(45.0);
        telemetry.Longitude.Should().Be(90.0);
        telemetry.Altitude.Should().Be(408.0);
        telemetry.Velocity.Should().Be(7.66);
    }

    [Fact]
    public void Create_WithOptionalParameters_ShouldSetAllProperties()
    {
        var telemetry = Telemetry.Create(
            _satelliteId, _timestamp, 45.0, 90.0, 408.0, 7.66,
            temperature: 20.5,
            batteryLevel: 85.0,
            signalStrength: -60.0);

        telemetry.Temperature.Should().Be(20.5);
        telemetry.BatteryLevel.Should().Be(85.0);
        telemetry.SignalStrength.Should().Be(-60.0);
    }

    [Fact]
    public void Create_OptionalParametersOmitted_ShouldBeNull()
    {
        var telemetry = Telemetry.Create(_satelliteId, _timestamp, 45.0, 90.0, 408.0, 7.66);

        telemetry.Temperature.Should().BeNull();
        telemetry.BatteryLevel.Should().BeNull();
        telemetry.SignalStrength.Should().BeNull();
    }

    [Fact]
    public void Create_EmptySatelliteId_ShouldThrowArgumentException()
    {
        Action act = () => Telemetry.Create(Guid.Empty, _timestamp, 45.0, 90.0, 408.0, 7.66);

        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().Be("satelliteId");
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(-200)]
    public void Create_InvalidLatitude_ShouldThrowArgumentOutOfRangeException(double latitude)
    {
        Action act = () => Telemetry.Create(_satelliteId, _timestamp, latitude, 90.0, 408.0, 7.66);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(360)]
    public void Create_InvalidLongitude_ShouldThrowArgumentOutOfRangeException(double longitude)
    {
        Action act = () => Telemetry.Create(_satelliteId, _timestamp, 45.0, longitude, 408.0, 7.66);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_NegativeAltitude_ShouldThrowArgumentOutOfRangeException(double altitude)
    {
        Action act = () => Telemetry.Create(_satelliteId, _timestamp, 45.0, 90.0, altitude, 7.66);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.001)]
    public void Create_NegativeVelocity_ShouldThrowArgumentOutOfRangeException(double velocity)
    {
        Action act = () => Telemetry.Create(_satelliteId, _timestamp, 45.0, 90.0, 408.0, velocity);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.1)]
    [InlineData(101)]
    [InlineData(200)]
    public void Create_InvalidBatteryLevel_ShouldThrowArgumentOutOfRangeException(double batteryLevel)
    {
        Action act = () => Telemetry.Create(
            _satelliteId, _timestamp, 45.0, 90.0, 408.0, 7.66, batteryLevel: batteryLevel);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_BoundaryBatteryLevel_ShouldCreateTelemetry()
    {
        var telemetry0 = Telemetry.Create(_satelliteId, _timestamp, 45.0, 90.0, 408.0, 7.66, batteryLevel: 0);
        var telemetry100 = Telemetry.Create(_satelliteId, _timestamp, 45.0, 90.0, 408.0, 7.66, batteryLevel: 100);

        telemetry0.BatteryLevel.Should().Be(0);
        telemetry100.BatteryLevel.Should().Be(100);
    }
}
