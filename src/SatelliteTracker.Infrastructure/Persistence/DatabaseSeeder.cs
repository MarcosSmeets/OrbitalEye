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
            // ── Space Stations ──
            Satellite.Create("ISS (ZARYA)", 25544, "1998-067A", "NASA/Roscosmos", new DateTime(1998, 11, 20, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("TIANGONG", 48274, "2021-035A", "CNSA", new DateTime(2021, 4, 29, 0, 0, 0, DateTimeKind.Utc)),

            // ── Telescopes & Science ──
            Satellite.Create("HUBBLE SPACE TELESCOPE", 20580, "1990-037B", "NASA", new DateTime(1990, 4, 24, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("TESS", 43435, "2018-038A", "NASA", new DateTime(2018, 4, 18, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("CHANDRA X-RAY OBSERVATORY", 25867, "1999-040B", "NASA", new DateTime(1999, 7, 23, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("DSCOVR", 40390, "2015-007A", "NOAA/NASA", new DateTime(2015, 2, 11, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("FERMI GAMMA-RAY SPACE TELESCOPE", 33053, "2008-029A", "NASA", new DateTime(2008, 6, 11, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SWIFT", 28485, "2004-047A", "NASA", new DateTime(2004, 11, 20, 0, 0, 0, DateTimeKind.Utc)),

            // ── Weather Satellites ──
            Satellite.Create("NOAA 19", 33591, "2009-005A", "NOAA", new DateTime(2009, 2, 6, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("NOAA 20 (JPSS-1)", 43013, "2017-073A", "NOAA", new DateTime(2017, 11, 18, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("NOAA 21 (JPSS-2)", 54234, "2022-150A", "NOAA", new DateTime(2022, 11, 10, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GOES 16", 41866, "2016-071A", "NOAA", new DateTime(2016, 11, 19, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GOES 17", 43226, "2018-022A", "NOAA", new DateTime(2018, 3, 1, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GOES 18", 51850, "2022-021A", "NOAA", new DateTime(2022, 3, 1, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("METOP-B", 38771, "2012-049A", "EUMETSAT", new DateTime(2012, 9, 17, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("METOP-C", 43689, "2018-087A", "EUMETSAT", new DateTime(2018, 11, 7, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SUOMI NPP", 37849, "2011-061A", "NASA/NOAA", new DateTime(2011, 10, 28, 0, 0, 0, DateTimeKind.Utc)),

            // ── Earth Observation ──
            Satellite.Create("TERRA", 25994, "1999-068A", "NASA", new DateTime(1999, 12, 18, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("AQUA", 27424, "2002-022A", "NASA", new DateTime(2002, 5, 4, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("LANDSAT 8", 39084, "2013-008A", "NASA/USGS", new DateTime(2013, 2, 11, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("LANDSAT 9", 49260, "2021-088A", "NASA/USGS", new DateTime(2021, 9, 27, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-1A", 39634, "2014-016A", "ESA", new DateTime(2014, 4, 3, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-2A", 40697, "2015-028A", "ESA", new DateTime(2015, 6, 23, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-2B", 42063, "2017-013A", "ESA", new DateTime(2017, 3, 7, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-3A", 41335, "2016-011A", "ESA", new DateTime(2016, 2, 16, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-3B", 43437, "2018-039A", "ESA", new DateTime(2018, 4, 25, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("SENTINEL-6A", 46984, "2020-086A", "ESA/NASA", new DateTime(2020, 11, 21, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("CRYOSAT 2", 36508, "2010-013A", "ESA", new DateTime(2010, 4, 8, 0, 0, 0, DateTimeKind.Utc)),

            // ── Navigation (GPS) ──
            Satellite.Create("GPS BIIR-2 (PRN 13)", 24876, "1997-035A", "USAF", new DateTime(1997, 7, 23, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GPS BIIF-1 (PRN 25)", 36585, "2010-022A", "USAF", new DateTime(2010, 5, 28, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GPS BIIF-12 (PRN 08)", 40730, "2015-033A", "USAF", new DateTime(2015, 7, 15, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GPS BIII-4 (PRN 18)", 48859, "2021-054A", "USAF", new DateTime(2021, 6, 17, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GPS BIII-5 (PRN 23)", 50521, "2022-003A", "USAF", new DateTime(2022, 1, 18, 0, 0, 0, DateTimeKind.Utc)),

            // ── Navigation (Galileo) ──
            Satellite.Create("GALILEO-FM1 (GSAT0101)", 37846, "2011-060A", "ESA", new DateTime(2011, 10, 21, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("GALILEO-FM10 (GSAT0210)", 40890, "2015-045B", "ESA", new DateTime(2015, 9, 11, 0, 0, 0, DateTimeKind.Utc)),

            // ── Communications (Starlink samples) ──
            Satellite.Create("STARLINK-1007", 44713, "2019-074A", "SpaceX", new DateTime(2019, 11, 11, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("STARLINK-1130", 44914, "2020-006A", "SpaceX", new DateTime(2020, 1, 29, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("STARLINK-2305", 47694, "2021-012A", "SpaceX", new DateTime(2021, 2, 4, 0, 0, 0, DateTimeKind.Utc)),

            // ── Communications (Iridium) ──
            Satellite.Create("IRIDIUM 106", 42803, "2017-039A", "Iridium", new DateTime(2017, 6, 25, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("IRIDIUM 112", 42805, "2017-039C", "Iridium", new DateTime(2017, 6, 25, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("IRIDIUM 160", 43928, "2019-002A", "Iridium", new DateTime(2019, 1, 11, 0, 0, 0, DateTimeKind.Utc)),

            // ── Communications (Other) ──
            Satellite.Create("GLOBALSTAR M087", 40269, "2014-062A", "Globalstar", new DateTime(2014, 10, 31, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("ORBCOMM FM116", 40086, "2014-033A", "ORBCOMM", new DateTime(2014, 7, 14, 0, 0, 0, DateTimeKind.Utc)),

            // ── Notable / Debris ──
            Satellite.Create("COSMOS 2251 DEB", 34427, "1993-036A", null, new DateTime(1993, 6, 16, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("VANGUARD 1", 5, "1958-002B", "US Navy", new DateTime(1958, 3, 17, 0, 0, 0, DateTimeKind.Utc)),

            // ── Japanese ──
            Satellite.Create("ALOS-2 (DAICHI-2)", 39766, "2014-029A", "JAXA", new DateTime(2014, 5, 24, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("HIMAWARI-8", 40267, "2014-060A", "JMA", new DateTime(2014, 10, 7, 0, 0, 0, DateTimeKind.Utc)),

            // ── Indian ──
            Satellite.Create("OCEANSAT-3", 54361, "2022-155A", "ISRO", new DateTime(2022, 11, 26, 0, 0, 0, DateTimeKind.Utc)),
            Satellite.Create("CARTOSAT-2S", 43111, "2018-004A", "ISRO", new DateTime(2018, 1, 12, 0, 0, 0, DateTimeKind.Utc)),
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
