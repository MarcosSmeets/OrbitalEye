using System.Globalization;

namespace SatelliteTracker.Infrastructure.External;

public static class TleParser
{
    public record TleData(
        int NoradId,
        double Inclination,
        double Eccentricity,
        double RightAscension,
        double ArgumentOfPerigee,
        double MeanAnomaly,
        double MeanMotion,
        DateTime Epoch);

    public static TleData Parse(string line1, string line2)
    {
        ArgumentNullException.ThrowIfNull(line1);
        ArgumentNullException.ThrowIfNull(line2);

        if (line1.Length < 69)
            throw new FormatException($"TLE line 1 must be at least 69 characters, got {line1.Length}.");

        if (line2.Length < 69)
            throw new FormatException($"TLE line 2 must be at least 69 characters, got {line2.Length}.");

        if (line1[0] != '1')
            throw new FormatException("TLE line 1 must start with '1'.");

        if (line2[0] != '2')
            throw new FormatException("TLE line 2 must start with '2'.");

        // TLE Format (0-indexed column positions):
        // Line 1:
        //   col 2-6:   NORAD catalog number
        //   col 18-19: epoch year (last 2 digits)
        //   col 20-31: epoch day of year (fractional)
        // Line 2:
        //   col 8-15:  inclination (degrees)
        //   col 17-24: RAAN (degrees)
        //   col 26-32: eccentricity (decimal point assumed, e.g., "0006703" = 0.0006703)
        //   col 34-41: argument of perigee (degrees)
        //   col 43-50: mean anomaly (degrees)
        //   col 52-62: mean motion (revs/day)

        var noradId = int.Parse(line1.Substring(2, 5).Trim(), CultureInfo.InvariantCulture);

        var epochYear = int.Parse(line1.Substring(18, 2).Trim(), CultureInfo.InvariantCulture);
        var epochDay = double.Parse(line1.Substring(20, 12).Trim(), CultureInfo.InvariantCulture);

        // TLE uses 2-digit year: 57-99 -> 1957-1999, 00-56 -> 2000-2056
        var fullYear = epochYear < 57 ? 2000 + epochYear : 1900 + epochYear;
        var epoch = new DateTime(fullYear, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(epochDay - 1);

        var inclination = double.Parse(line2.Substring(8, 8).Trim(), CultureInfo.InvariantCulture);
        var raan = double.Parse(line2.Substring(17, 8).Trim(), CultureInfo.InvariantCulture);
        var eccentricity = double.Parse("0." + line2.Substring(26, 7).Trim(), CultureInfo.InvariantCulture);
        var argPerigee = double.Parse(line2.Substring(34, 8).Trim(), CultureInfo.InvariantCulture);
        var meanAnomaly = double.Parse(line2.Substring(43, 8).Trim(), CultureInfo.InvariantCulture);
        var meanMotion = double.Parse(line2.Substring(52, 11).Trim(), CultureInfo.InvariantCulture);

        return new TleData(noradId, inclination, eccentricity, raan, argPerigee, meanAnomaly, meanMotion, epoch);
    }
}
