using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HotChocolate.Checker.Persistence;

public class DesignTimeDatabaseContextFactory : IDesignTimeDbContextFactory<CheckerDbContext>
{
    public CheckerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(System.IO.Path.GetFullPath("../HotChocolate.Checker/appsettings.json"))
            .Build();
        var connectionString = configuration.GetConnectionString("Checker");
        Console.WriteLine(connectionString);
        var optionsBuilder = new DbContextOptionsBuilder<CheckerDbContext>();
        optionsBuilder.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString));

        return new(optionsBuilder.Options);
    }
}