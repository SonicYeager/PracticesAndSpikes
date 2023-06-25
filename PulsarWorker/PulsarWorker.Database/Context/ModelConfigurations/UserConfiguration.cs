using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulsarWorker.Data.Entities;

namespace PulsarWorker.Database.Context.ModelConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(static k => k.Id);

        builder.HasData(new UserEntity
        {
            Id = 1,
            Name = "PulsarWorker",
            Surname = "Test",
        });
    }
}