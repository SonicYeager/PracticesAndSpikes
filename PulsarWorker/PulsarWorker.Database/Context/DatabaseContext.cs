using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PulsarWorker.Data.Entities;

namespace PulsarWorker.Database.Context;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.GetFullPath("../PulsarWorker/appsettings.json"))
            .Build();
        var connectionString = configuration.GetConnectionString("PulsarWorker");
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PulsarMessageEntity>(cnf =>
        {
            cnf.HasKey(k => k.MessageId);

            cnf.Property(p => p.ReceivedAt)
                .ValueGeneratedOnAdd();
        });
    }
}