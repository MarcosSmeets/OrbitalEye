using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Infrastructure.Persistence.Configurations;

public class TelemetryConfiguration : IEntityTypeConfiguration<Telemetry>
{
    public void Configure(EntityTypeBuilder<Telemetry> builder)
    {
        builder.ToTable("telemetry");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id");

        builder.Property(t => t.SatelliteId)
            .HasColumnName("satellite_id");

        builder.Property(t => t.Timestamp)
            .HasColumnName("timestamp");

        builder.Property(t => t.Latitude)
            .HasColumnName("latitude");

        builder.Property(t => t.Longitude)
            .HasColumnName("longitude");

        builder.Property(t => t.Altitude)
            .HasColumnName("altitude");

        builder.Property(t => t.Velocity)
            .HasColumnName("velocity");

        builder.Property(t => t.Temperature)
            .HasColumnName("temperature");

        builder.Property(t => t.BatteryLevel)
            .HasColumnName("battery_level");

        builder.Property(t => t.SignalStrength)
            .HasColumnName("signal_strength");

        builder.HasIndex(t => new { t.SatelliteId, t.Timestamp })
            .IsDescending(false, true);

        builder.HasOne<Satellite>()
            .WithMany()
            .HasForeignKey(t => t.SatelliteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
