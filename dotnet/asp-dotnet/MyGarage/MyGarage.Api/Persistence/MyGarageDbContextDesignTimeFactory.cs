using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyGarage.Api.Persistence;

public class MyGarageDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MyGarageDbContext>
{
    /// <inheritdoc />
    public MyGarageDbContext CreateDbContext(string[] args)
    {
        const string connectionString = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
        var options = new DbContextOptionsBuilder<MyGarageDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        return new(options);
    }
}