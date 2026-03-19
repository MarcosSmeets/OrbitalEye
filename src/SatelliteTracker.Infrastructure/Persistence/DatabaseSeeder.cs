using SatelliteTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SatelliteTracker.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SatelliteTrackerDbContext context)
    {
        await SeedSatellitesAsync(context);
        await SeedGroundStationsAsync(context);
    }

    private static async Task SeedSatellitesAsync(SatelliteTrackerDbContext context)
    {
        if (await context.Satellites.AnyAsync())
            return;

        var satellites = new[]
        {
            Satellite.Create("ISS (ZARYA)", 25544, "1998-067A", "NASA/Roscosmos", new DateTime(1998, 11, 20, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("HUBBLE SPACE TELESCOPE", 20580, "1990-037B", "NASA", new DateTime(1990, 4, 24, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("STARLINK-1007", 44713, "2019-074A", "SpaceX", new DateTime(2019, 11, 11, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("NOAA 19", 33591, "2009-005A", "NOAA", new DateTime(2009, 2, 6, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("TERRA", 25994, "1999-068A", "NASA", new DateTime(1999, 12, 18, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GPS BIIR-2 (PRN 13)", 24876, "1997-035A", "USAF", new DateTime(1997, 7, 23, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GOES 16", 41866, "2016-071A", "NOAA", new DateTime(2016, 11, 19, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-6A", 46984, "2020-086A", "ESA/NASA", new DateTime(2020, 11, 21, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("COSMOS 2251 DEB", 34427, "1993-036A", null, new DateTime(1993, 6, 16, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("TIANGONG", 48274, "2021-035A", "CNSA", new DateTime(2021, 4, 29, 0, 0, 0, DateTimeKind.Utc)),
        };

        context.Satellites.AddRange(satellites);
        await context.SaveChangesAsync();
    }

    private static async Task SeedGroundStationsAsync(SatelliteTrackerDbContext context)
    {
        if (await context.GroundStations.AnyAsync())
            return;

        var groundStations = new[]
        {
            GroundStation.Create("NASA Goddard", 38.99, -76.85, 53.0, "United States"),
            GroundStation.Create("ESA Darmstadt", 49.87, 8.63, 144.0, "Germany"),
        };

        context.GroundStations.AddRange(groundStations);
        await context.SaveChangesAsync();
    }
}
