using Microsoft.EntityFrameworkCore;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Interfaces;

namespace SatelliteTracker.Infrastructure.Persistence;

public class SatelliteTrackerDbContext : DbContext, IUnitOfWork
{
    public DbSet<Satellite> Satellites => Set<Satellite>();
    public DbSet<Orbit> Orbits => Set<Orbit>();
    public DbSet<Telemetry> Telemetries => Set<Telemetry>();
    public DbSet<GroundStation> GroundStations => Set<GroundStation>();

    public SatelliteTrackerDbContext(DbContextOptions<SatelliteTrackerDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SatelliteTrackerDbContext).Assembly);
    }
}
