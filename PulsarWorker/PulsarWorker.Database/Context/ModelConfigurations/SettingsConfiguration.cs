using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulsarWorker.Data.Entities;

namespace PulsarWorker.Database.Context.ModelConfigurations;

public class SettingsConfiguration : IEntityTypeConfiguration<SettingsEntity>
{
    public void Configure(EntityTypeBuilder<SettingsEntity> builder)
    {
        builder.HasKey(static k => k.Id);

        builder.HasOne<UserEntity>()
            .WithMany()
            .HasForeignKey(static s => s.UserId);

        builder.HasData(new List<SettingsEntity>
        {
            new()
            {
                Id = 1,
                Key = "Pulsar Host",
                Value = "",
                UserId = 1,
            },
            new()
            {
                Id = 2,
                Key = "App Theme",
                Value = "Dark",
                UserId = 1,
            },
        });
    }
}