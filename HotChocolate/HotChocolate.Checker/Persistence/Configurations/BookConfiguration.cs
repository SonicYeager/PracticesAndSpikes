using Bogus;
using HotChocolate.Checker.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotChocolate.Checker.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<BookEntity>
{
    public void Configure(EntityTypeBuilder<BookEntity> builder)
    {
        builder.HasKey(static k => k.Id);

        builder.HasOne(static p => p.Author)
            .WithMany();

        var books = new List<BookEntity>();
        for (var i = 1; i < 100; i++)
        {
            var faker = new Faker();
            books.Add(new()
            {
                Id = i,
                AuthorId = i,
                Genre = faker.Music.Genre(),
                Language = faker.Random.Word(),
                Title = faker.Company.CompanyName(),
                PageCount = faker.Random.Int(10, 9999),
                ISBN = faker.Commerce.Ean13(),
                PublicationDate = faker.Date.Past(),
            });
        }

        builder.HasData(books);
    }
}