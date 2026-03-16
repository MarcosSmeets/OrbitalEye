using SatelliteTracker.Application.DTOs;
using SatelliteTracker.Application.Mapping;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;
using System.Globalization;

namespace SatelliteTracker.Application.UseCases;

public class UpdateOrbitFromTleUseCase
{
    private readonly ISatelliteRepository _satelliteRepository;
    private readonly IOrbitRepository _orbitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrbitFromTleUseCase(
        ISatelliteRepository satelliteRepository,
        IOrbitRepository orbitRepository,
        IUnitOfWork unitOfWork)
    {
        _satelliteRepository = satelliteRepository;
        _orbitRepository = orbitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OrbitDto?> ExecuteAsync(int noradId, string tleLine1, string tleLine2, CancellationToken cancellationToken = default)
    {
        var satellite = await _satelliteRepository.GetByNoradIdAsync(noradId, cancellationToken);

        if (satellite is null)
            return null;

        var (epoch, inclination, eccentricity, rightAscension, argumentOfPerigee, meanAnomaly, meanMotion) =
            ParseTle(tleLine1, tleLine2);

        var orbit = Orbit.FromTle(
            satellite.Id,
            inclination,
            eccentricity,
            rightAscension,
            argumentOfPerigee,
            meanAnomaly,
            meanMotion,
            epoch,
            tleLine1,
            tleLine2);

        await _orbitRepository.AddAsync(orbit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return SatelliteMapper.ToDto(orbit);
    }

    private static (DateTime Epoch, double Inclination, double Eccentricity, double RightAscension,
        double ArgumentOfPerigee, double MeanAnomaly, double MeanMotion) ParseTle(string line1, string line2)
    {
        // TLE Line 1: Epoch is at columns 18-32 (0-indexed)
        // Format: YYDDD.DDDDDDDD (year + fractional day of year)
        var epochStr = line1.Substring(18, 14).Trim();
        var epoch = ParseTleEpoch(epochStr);

        // TLE Line 2 fields (0-indexed column positions):
        // Inclination:         8-16
        // RAAN:               17-25
        // Eccentricity:       26-33 (implicit leading decimal point)
        // Arg of Perigee:     34-42
        // Mean Anomaly:       43-51
        // Mean Motion:        52-63
        var inclination = double.Parse(line2.Substring(8, 8).Trim(), CultureInfo.InvariantCulture);
        var rightAscension = double.Parse(line2.Substring(17, 8).Trim(), CultureInfo.InvariantCulture);
        var eccentricity = double.Parse("0." + line2.Substring(26, 7).Trim(), CultureInfo.InvariantCulture);
        var argumentOfPerigee = double.Parse(line2.Substring(34, 8).Trim(), CultureInfo.InvariantCulture);
        var meanAnomaly = double.Parse(line2.Substring(43, 8).Trim(), CultureInfo.InvariantCulture);
        var meanMotion = double.Parse(line2.Substring(52, 11).Trim(), CultureInfo.InvariantCulture);

        return (epoch, inclination, eccentricity, rightAscension, argumentOfPerigee, meanAnomaly, meanMotion);
    }

    private static DateTime ParseTleEpoch(string epochStr)
    {
        var year = int.Parse(epochStr.Substring(0, 2), CultureInfo.InvariantCulture);
        var dayOfYear = double.Parse(epochStr.Substring(2), CultureInfo.InvariantCulture);

        // TLE uses 2-digit year: 57-99 -> 1957-1999, 00-56 -> 2000-2056
        year = year >= 57 ? 1900 + year : 2000 + year;

        var epoch = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddDays(dayOfYear - 1);

        return epoch;
    }
}
