using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Persistence.ModelConfigurations;

public class GarageConfiguration : IEntityTypeConfiguration<Garage>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Garage> builder)
    {
        builder.HasKey(static g => g.Id);

        builder.ToTable("Garages");
    }
}