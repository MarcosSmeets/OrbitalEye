using SGPdotNET.CoordinateSystem;
using SGPdotNET.Propagation;
using SGPdotNET.TLE;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Infrastructure.External;

public class Sgp4OrbitPropagator : IOrbitPropagator
{
    public (double Latitude, double Longitude, double Altitude) CalculatePosition(
        Domain.Entities.Orbit orbit, DateTime atTime)
    {
        // Reconstruct TLE-like data from orbital parameters to create a synthetic TLE
        // This is approximate - for best results, use CalculatePositionFromTle with actual TLE lines
        var tleLine1 = GenerateTleLine1(orbit);
        var tleLine2 = GenerateTleLine2(orbit);

        var result = CalculatePositionFromTle(tleLine1, tleLine2, atTime);
        return (result.Latitude, result.Longitude, result.Altitude);
    }

    public (double Latitude, double Longitude, double Altitude, double Velocity) CalculatePositionFromTle(
        string tleLine1, string tleLine2, DateTime atTime)
    {
        var tle = new Tle(tleLine1, tleLine2);
        var sgp4 = new Sgp4(tle);

        var eci = sgp4.FindPosition(atTime);
        var geo = eci.ToGeodetic();

        var latitude = geo.Latitude.Degrees;
        var longitude = geo.Longitude.Degrees;
        var altitude = geo.Altitude; // already in km

        // Calculate velocity magnitude from ECI velocity vector
        var velocity = Math.Sqrt(
            eci.Velocity.X * eci.Velocity.X +
            eci.Velocity.Y * eci.Velocity.Y +
            eci.Velocity.Z * eci.Velocity.Z);

        return (latitude, longitude, altitude, velocity);
    }

    private static string GenerateTleLine1(Domain.Entities.Orbit orbit)
    {
        // Generate a synthetic TLE Line 1 from orbital parameters
        var epoch = orbit.Epoch;
        var yearSuffix = epoch.Year % 100;
        var dayOfYear = epoch.DayOfYear + (epoch.Hour + epoch.Minute / 60.0 + epoch.Second / 3600.0) / 24.0;

        // Format: 1 NNNNNC NNNNNAAA NNNNN.NNNNNNNN +.NNNNNNNN +NNNNN-N +NNNNN-N N NNNNN
        // Simplified format with zeros for unknown fields
        var line = $"1 00000U 00000A   {yearSuffix:00}{dayOfYear:000.00000000}  .00000000  00000-0  00000-0 0  0000";

        // Add checksum
        var checksum = CalculateChecksum(line);
        return line + checksum;
    }

    private static string GenerateTleLine2(Domain.Entities.Orbit orbit)
    {
        // Format: 2 NNNNN NNN.NNNN NNN.NNNN NNNNNNN NNN.NNNN NNN.NNNN NN.NNNNNNNNNNNNNN
        var eccentricityStr = (orbit.Eccentricity * 10000000).ToString("0000000");

        var line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
            "2 00000 {0,8:F4} {1,8:F4} {2} {3,8:F4} {4,8:F4} {5,11:F8}00000",
            orbit.Inclination, orbit.RightAscension, eccentricityStr,
            orbit.ArgumentOfPerigee, orbit.MeanAnomaly, orbit.MeanMotion);

        var checksum = CalculateChecksum(line);
        return line + checksum;
    }

    private static int CalculateChecksum(string line)
    {
        var sum = 0;
        foreach (var c in line)
        {
            if (c >= '0' && c <= '9')
                sum += c - '0';
            else if (c == '-')
                sum += 1;
        }
        return sum % 10;
    }
}
