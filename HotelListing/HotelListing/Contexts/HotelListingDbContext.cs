using HotelListing.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Contexts;

public sealed class HotelListingDbContext : DbContext
{
    public HotelListingDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CountryEntity>().HasData(
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
        modelBuilder.Entity<HotelEntity>().HasData(
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

    public DbSet<HotelEntity> Hotels { get; set; }
    public DbSet<CountryEntity> Countries { get; set; }
}