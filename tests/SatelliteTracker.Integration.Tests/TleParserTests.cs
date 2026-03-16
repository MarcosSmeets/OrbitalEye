using FluentAssertions;
using SatelliteTracker.Infrastructure.External;

namespace SatelliteTracker.Integration.Tests;

public class TleParserTests
{
    // ISS (ZARYA) - real TLE data
    private const string IssLine1 = "1 25544U 98067A   24045.51948398  .00015753  00000+0  28215-3 0  9993";
    private const string IssLine2 = "2 25544  51.6412 290.4299 0004703 103.8983 256.2596 15.50080511440584";

    // Hubble Space Telescope - real TLE data
    private const string HubbleLine1 = "1 20580U 90037B   24045.18163980  .00001148  00000+0  57957-4 0  9996";
    private const string HubbleLine2 = "2 20580  28.4698  52.3425 0002678 125.4451 234.6431 15.09443792527508";

    [Fact]
    public void Parse_IssData_ReturnsCorrectNoradId()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.NoradId.Should().Be(25544);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectInclination()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.Inclination.Should().BeApproximately(51.6412, 0.01);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectEccentricity()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.Eccentricity.Should().BeApproximately(0.0004703, 0.0000001);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectMeanMotion()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.MeanMotion.Should().BeApproximately(15.50080511, 0.01);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectRightAscension()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.RightAscension.Should().BeApproximately(290.4299, 0.01);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectArgumentOfPerigee()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.ArgumentOfPerigee.Should().BeApproximately(103.8983, 0.01);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectMeanAnomaly()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        result.MeanAnomaly.Should().BeApproximately(256.2596, 0.01);
    }

    [Fact]
    public void Parse_IssData_ReturnsCorrectEpoch()
    {
        var result = TleParser.Parse(IssLine1, IssLine2);

        // Epoch year 24 -> 2024, day 045.51948398
        result.Epoch.Year.Should().Be(2024);
        result.Epoch.Kind.Should().Be(DateTimeKind.Utc);
        // Day 45 of 2024 = February 14
        result.Epoch.Month.Should().Be(2);
        result.Epoch.Day.Should().Be(14);
    }

    [Fact]
    public void Parse_HubbleData_ReturnsCorrectNoradId()
    {
        var result = TleParser.Parse(HubbleLine1, HubbleLine2);

        result.NoradId.Should().Be(20580);
    }

    [Fact]
    public void Parse_HubbleData_ReturnsCorrectInclination()
    {
        var result = TleParser.Parse(HubbleLine1, HubbleLine2);

        result.Inclination.Should().BeApproximately(28.4698, 0.01);
    }

    [Fact]
    public void Parse_HubbleData_ReturnsCorrectEccentricity()
    {
        var result = TleParser.Parse(HubbleLine1, HubbleLine2);

        result.Eccentricity.Should().BeApproximately(0.0002678, 0.0000001);
    }

    [Fact]
    public void Parse_HubbleData_ReturnsCorrectMeanMotion()
    {
        var result = TleParser.Parse(HubbleLine1, HubbleLine2);

        result.MeanMotion.Should().BeApproximately(15.09443792, 0.01);
    }

    [Fact]
    public void Parse_LineTooShort_ThrowsFormatException()
    {
        var shortLine = "1 25544U";

        var act = () => TleParser.Parse(shortLine, IssLine2);

        act.Should().Throw<FormatException>().WithMessage("*at least 69*");
    }

    [Fact]
    public void Parse_InvalidLinePrefix_ThrowsFormatException()
    {
        var badLine1 = "2" + IssLine1.Substring(1);

        var act = () => TleParser.Parse(badLine1, IssLine2);

        act.Should().Throw<FormatException>().WithMessage("*must start with '1'*");
    }

    [Fact]
    public void Parse_NullLine1_ThrowsArgumentNullException()
    {
        var act = () => TleParser.Parse(null!, IssLine2);

        act.Should().Throw<ArgumentNullException>();
    }
}
