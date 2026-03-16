using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelliteTracker.Domain.Entities;

namespace SatelliteTracker.Infrastructure.Persistence.Configurations;

public class GroundStationConfiguration : IEntityTypeConfiguration<GroundStation>
{
    public void Configure(EntityTypeBuilder<GroundStation> builder)
    {
        builder.ToTable("ground_stations");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasColumnName("id");

        builder.Property(g => g.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Latitude)
            .HasColumnName("latitude");

        builder.Property(g => g.Longitude)
            .HasColumnName("longitude");

        builder.Property(g => g.Altitude)
            .HasColumnName("altitude");

        builder.Property(g => g.Country)
            .HasColumnName("country")
            .IsRequired();
    }
}
