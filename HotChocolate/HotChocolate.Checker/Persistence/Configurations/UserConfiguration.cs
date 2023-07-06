using Bogus;
using HotChocolate.Checker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotChocolate.Checker.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(static k => k.Id);

        builder.HasMany<BookEntity>()
            .WithOne(static p => p.Author);

        var users = new List<UserEntity>();
        for (var i = 1; i < 100; i++)
        {
            var faker = new Faker();
            users.Add(new()
            {
                Id = i, Name = faker.Person.FirstName, SurName = faker.Person.LastName,
            });
        }

        builder.HasData(users);
    }
}