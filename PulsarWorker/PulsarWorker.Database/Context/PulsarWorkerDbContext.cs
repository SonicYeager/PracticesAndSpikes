using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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