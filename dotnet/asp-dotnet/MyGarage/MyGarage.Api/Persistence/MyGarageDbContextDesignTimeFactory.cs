using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyGarage.Api.Persistence;

public sealed class MyGarageDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MyGarageDbContext>
{
    /// <inheritdoc />
    public MyGarageDbContext CreateDbContext(string[] args)
    {
        //const string connectionStringMariaDb = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
        //var options = new DbContextOptionsBuilder<MyGarageDbContext>()
        //    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        //    .Options;

        const string connectionStringPostgreSql = "server=localhost;database=mygarage;username=root;password=my-secret;port=5432;";
        var options = new DbContextOptionsBuilder<MyGarageDbContext>()
            .UseNpgsql(connectionStringPostgreSql)
            .Options;
        return new(options);
    }
}