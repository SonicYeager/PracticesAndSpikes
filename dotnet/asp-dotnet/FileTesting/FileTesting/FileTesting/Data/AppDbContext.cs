using FileTesting.Models;
using Microsoft.EntityFrameworkCore;

namespace FileTesting.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<ImageMetadata> ImageMetadata { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ImageMetadata>(static entity =>
        {
            entity.HasKey(static e => e.Id);
            entity.Property(static e => e.ScalingMethod).HasMaxLength(50);
            entity.Property(static e => e.OriginalMinioPath).HasMaxLength(512).IsRequired();
            entity.Property(static e => e.ScaledMinioPath).HasMaxLength(512).IsRequired();
        });
    }
}