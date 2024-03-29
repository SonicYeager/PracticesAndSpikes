﻿using HotelListing.Api.Data.Entities;
using HotelListing.Api.Data.Models.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Data.Contexts
{
    public sealed class HotelListingDbContext : IdentityDbContext<UserEntity>
    {
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new CountryConfiguration());
            modelBuilder.ApplyConfiguration(new HotelConfiguration());
        }

        public DbSet<HotelEntity> Hotels { get; set; }
        public DbSet<CountryEntity> Countries { get; set; }
    }
}