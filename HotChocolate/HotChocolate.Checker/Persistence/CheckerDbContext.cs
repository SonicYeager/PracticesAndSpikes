using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Checker.Persistence;

public sealed class CheckerDbContext : DbContext
{
    public CheckerDbContext(DbContextOptions<CheckerDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}