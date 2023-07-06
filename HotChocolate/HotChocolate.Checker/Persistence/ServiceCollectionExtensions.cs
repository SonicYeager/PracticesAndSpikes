using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace HotChocolate.Checker.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection service, string connectionString)
    {
        return service
            .AddSingleton<DbContextOptions<CheckerDbContext>>(_ => new DbContextOptionsBuilder<CheckerDbContext>()
                .UseLoggerFactory(LoggerFactory.Create(static builder =>
                {
                    builder
                        .AddDebug()
                        .AddConsole();
                }))
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .Options)
            .AddDbContextPool<CheckerDbContext>(o => o
                .UseLoggerFactory(LoggerFactory.Create(static builder =>
                {
                    builder
                        .AddDebug()
                        .AddConsole();
                }))
                .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }
}