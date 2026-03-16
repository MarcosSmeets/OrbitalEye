using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Infrastructure.Persistence.Configurations;

public class OrbitConfiguration : IEntityTypeConfiguration<Orbit>
{
    public void Configure(EntityTypeBuilder<Orbit> builder)
    {
        builder.ToTable("orbits");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id");

        builder.Property(o => o.SatelliteId)
            .HasColumnName("satellite_id");

        builder.Property(o => o.Inclination)
            .HasColumnName("inclination");

        builder.Property(o => o.Eccentricity)
            .HasColumnName("eccentricity");

        builder.Property(o => o.RightAscension)
            .HasColumnName("raan");

        builder.Property(o => o.ArgumentOfPerigee)
            .HasColumnName("argument_of_perigee");

        builder.Property(o => o.MeanAnomaly)
            .HasColumnName("mean_anomaly");

        builder.Property(o => o.MeanMotion)
            .HasColumnName("mean_motion");

        builder.Property(o => o.Epoch)
            .HasColumnName("epoch");

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(o => o.SatelliteId);

        builder.HasOne<Satellite>()
            .WithMany()
            .HasForeignKey(o => o.SatelliteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
