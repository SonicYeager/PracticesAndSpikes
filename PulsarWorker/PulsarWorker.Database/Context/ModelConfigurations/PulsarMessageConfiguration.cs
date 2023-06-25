using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulsarWorker.Data.Entities;

namespace PulsarWorker.Database.Context.ModelConfigurations;

public class PulsarMessageConfiguration : IEntityTypeConfiguration<PulsarMessageEntity>
{
    public void Configure(EntityTypeBuilder<PulsarMessageEntity> builder)
    {
        builder.HasKey(static k => k.Id);

        builder.Property(static p => p.ReceivedAt)
            .ValueGeneratedOnAdd();
    }
}