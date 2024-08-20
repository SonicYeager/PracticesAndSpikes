using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Persistence.ModelConfigurations;

public sealed class FuelStopConfiguration : IEntityTypeConfiguration<FuelStop>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FuelStop> builder)
    {
        builder.HasKey(static g => g.Id);

        builder.Property(static p => p.Note)
            .HasMaxLength(2048);

        builder.ToTable("FuelStops");
    }
}