using FluentAssertions;
using SatelliteTracker.Infrastructure.External;

namespace SatelliteTracker.Integration.Tests;

public class Sgp4OrbitPropagatorTests
{
    // ISS (ZARYA) - real TLE data
    private const string IssLine1 = "1 25544U 98067A   24045.51948398  .00015753  00000+0  28215-3 0  9993";
    private const string IssLine2 = "2 25544  51.6412 290.4299 0004703 103.8983 256.2596 15.50080511440584";

    // Hubble Space Telescope - real TLE data
    private const string HubbleLine1 = "1 20580U 90037B   24045.18163980  .00001148  00000+0  57957-4 0  9996";
    private const string HubbleLine2 = "2 20580  28.4698  52.3425 0002678 125.4451 234.6431 15.09443792527508";

    // ISS TLE epoch: day 45.51948398 of 2024 = February 14, 2024 ~12:28 UTC
    private static readonly DateTime IssEpoch = new(2024, 2, 14, 12, 28, 0, DateTimeKind.Utc);

    // Hubble TLE epoch: day 45.18163980 of 2024 = February 14, 2024 ~04:21 UTC
    private static readonly DateTime HubbleEpoch = new(2024, 2, 14, 4, 21, 0, DateTimeKind.Utc);

    private readonly Sgp4OrbitPropagator _propagator = new();

    [Fact]
    public void CalculatePositionFromTle_WithIssTle_ReturnsValidPosition()
    {
        var result = _propagator.CalculatePositionFromTle(IssLine1, IssLine2, IssEpoch);

        result.Latitude.Should().BeInRange(-90, 90);
        result.Longitude.Should().BeInRange(-180, 180);
        result.Altitude.Should().BeGreaterThan(300);
        result.Velocity.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CalculatePositionFromTle_WithIssTle_ReturnsReasonableAltitude()
    {
        var result = _propagator.CalculatePositionFromTle(IssLine1, IssLine2, IssEpoch);

        result.Altitude.Should().BeInRange(300, 500,
            "the ISS orbits at approximately 400 km altitude");
    }

    [Fact]
    public void CalculatePositionFromTle_WithIssTle_ReturnsReasonableVelocity()
    {
        var result = _propagator.CalculatePositionFromTle(IssLine1, IssLine2, IssEpoch);

        result.Velocity.Should().BeInRange(7.5, 8.0,
            "the ISS in LEO travels at approximately 7.66 km/s");
    }

    [Fact]
    public void CalculatePositionFromTle_AtDifferentTimes_ReturnsDifferentPositions()
    {
        var atEpoch = _propagator.CalculatePositionFromTle(IssLine1, IssLine2, IssEpoch);
        var tenMinutesLater = _propagator.CalculatePositionFromTle(IssLine1, IssLine2, IssEpoch.AddMinutes(10));

        // The ISS moves ~4,600 km in 10 minutes, so positions must differ significantly
        var latDiff = Math.Abs(atEpoch.Latitude - tenMinutesLater.Latitude);
        var lonDiff = Math.Abs(atEpoch.Longitude - tenMinutesLater.Longitude);

        (latDiff + lonDiff).Should().BeGreaterThan(0.1,
            "the ISS should have moved noticeably in 10 minutes");
    }

    [Fact]
    public void CalculatePositionFromTle_WithHubbleTle_ReturnsValidPosition()
    {
        var result = _propagator.CalculatePositionFromTle(HubbleLine1, HubbleLine2, HubbleEpoch);

        result.Latitude.Should().BeInRange(-90, 90);
        result.Longitude.Should().BeInRange(-180, 180);
        result.Altitude.Should().BeInRange(500, 600,
            "Hubble orbits at approximately 540 km altitude");
        result.Velocity.Should().BeInRange(7.0, 8.0,
            "Hubble in LEO travels at approximately 7.59 km/s");
    }
}
