using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HotChocolatePoC.Database.Context
{
    internal class ArticlesDbContextFactory : IDesignTimeDbContextFactory<ArticlesDbContext>
    {
        /// <summary>
        /// Required for usage of EntityFramework-CLI-tools, e.g.'dotnet ef database update'
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public ArticlesDbContext CreateDbContext(string[] args)
        {
            IConfiguration conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ArticlesDbContext>();
            var connectionString = conf.GetConnectionString("HotelListingDbConnectionString");
            var version = ServerVersion.AutoDetect(connectionString);
            optionsBuilder.UseMySql(connectionString, version);
            return new ArticlesDbContext(optionsBuilder.Options);
        }
    }
}