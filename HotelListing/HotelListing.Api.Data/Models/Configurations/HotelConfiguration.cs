using HotelListing.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Data.Models.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<HotelEntity>
    {
        public void Configure(EntityTypeBuilder<HotelEntity> builder)
        {
            builder.HasData(
                new HotelEntity
                {
                    Id = 1,
                    Name = "Achat Hotels",
                    Address = "Dresden - Leuben",
                    CountryId = 2,
                    Rating = 2.7
                },
                new HotelEntity
                {
                    Id = 2,
                    Name = "Hotel Omi",
                    Address = "Aquas Calientes",
                    CountryId = 1,
                    Rating = 4.7
                }
            );
        }
    }
}