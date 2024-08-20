using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Persistence.ModelConfigurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(static g => g.Id);

        builder.Property(static g => g.LicensePlate)
            .HasMaxLength(10);

        builder.HasMany(static p => p.FuelStops)
            .WithOne()
            .HasForeignKey(static f => f.VehicleId);

        builder.ToTable("Vehicles");
    }
}