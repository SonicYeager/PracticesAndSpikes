using FindACoach.Records;
using Microsoft.EntityFrameworkCore;

namespace FindACoach.Persistence;

public class FindACoachDbContext : DbContext
{
    public FindACoachDbContext(DbContextOptions<FindACoachDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Coach>(static e => e.ToTable("Coaches"));
    }
}