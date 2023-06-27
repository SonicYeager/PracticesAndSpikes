using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace PulsarWorker.Database.Context;

public class PulsarWorkerDbContext : DbContext
{
    public PulsarWorkerDbContext(DbContextOptions<PulsarWorkerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}