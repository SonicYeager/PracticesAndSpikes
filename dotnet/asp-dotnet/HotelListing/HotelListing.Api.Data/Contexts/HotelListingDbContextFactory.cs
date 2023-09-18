using HotelListing.Api.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class HotelListingDbContextFactory : IDesignTimeDbContextFactory<HotelListingDbContext>
{
    public HotelListingDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine(Path.GetFullPath("../HotelListing/appsettings.json"));

        IConfiguration conf = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.GetFullPath("../HotelListing/appsettings.json"), false, true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<HotelListingDbContext>();
        var connectionString = conf.GetConnectionString("HotelListingDbConnectionString");
        var version = ServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, version);
        return new HotelListingDbContext(optionsBuilder.Options);
    }
}