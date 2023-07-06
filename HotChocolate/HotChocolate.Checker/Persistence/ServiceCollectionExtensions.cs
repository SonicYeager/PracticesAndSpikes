using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Checker.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection service, string connectionString)
    {
        return service
            .AddSingleton<DbContextOptions<CheckerDbContext>>(_ => new DbContextOptionsBuilder<CheckerDbContext>()
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options);
    }
}