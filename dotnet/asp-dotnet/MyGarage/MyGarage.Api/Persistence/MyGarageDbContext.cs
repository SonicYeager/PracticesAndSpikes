using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Persistence.ModelConfigurations;

namespace MyGarage.Api.Persistence;

public class MyGarageDbContext : DbContext
{
    public MyGarageDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(GarageConfiguration))!);
    }
}