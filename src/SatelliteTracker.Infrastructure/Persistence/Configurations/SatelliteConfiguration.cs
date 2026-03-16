using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelliteTracker.Domain.Entities;
using SatelliteTracker.Domain.Enums;

namespace SatelliteTracker.Infrastructure.Persistence.Configurations;

public class SatelliteConfiguration : IEntityTypeConfiguration<Satellite>
{
    public void Configure(EntityTypeBuilder<Satellite> builder)
    {
        builder.ToTable("satellites");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("id");

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.NoradId)
            .HasColumnName("norad_id");

        builder.Property(s => s.InternationalDesignator)
            .HasColumnName("international_designator");

        builder.Property(s => s.LaunchDate)
            .HasColumnName("launch_date");

        builder.Property(s => s.Operator)
            .HasColumnName("operator");

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<string>();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at");

        builder.HasIndex(s => s.NoradId)
            .IsUnique();
    }
}
