using HotChocolate.Checker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotChocolate.Checker.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(static k => k.Id);

        builder.HasData(new UserEntity
        {
            Id = 1,
            Name = "PulsarWorker",
            SurName = "Test",
        });
    }
}