using HotelListing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Models.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<CountryEntity>
    {
        public void Configure(EntityTypeBuilder<CountryEntity> builder)
        {
            builder.HasData(
                new CountryEntity
                {
                    Id = 1,
                    Name = "Mexico",
                    ShortName = "MX"
                },
                new CountryEntity
                {
                    Id = 2,
                    Name = "Germany",
                    ShortName = "DE"
                }
            );
        }
    }
}