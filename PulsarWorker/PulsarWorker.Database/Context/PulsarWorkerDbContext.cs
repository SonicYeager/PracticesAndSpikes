using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace PulsarWorker.Database.Context;

public sealed class PulsarWorkerDbContext : DbContext
{
    public PulsarWorkerDbContext(DbContextOptions<PulsarWorkerDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}