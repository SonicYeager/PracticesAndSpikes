using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PulsarWorker.Database.Context;

public class DesignTimeDatabaseContextFactory : IDesignTimeDbContextFactory<PulsarWorkerDbContext>
{
    public PulsarWorkerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.GetFullPath("../PulsarWorker/appsettings.json"))
            .Build();
        var connectionString = configuration.GetConnectionString("PulsarWorker");
        Console.WriteLine(connectionString);
        var optionsBuilder = new DbContextOptionsBuilder<PulsarWorkerDbContext>();
        optionsBuilder.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString));

        return new(optionsBuilder.Options);
    }
}