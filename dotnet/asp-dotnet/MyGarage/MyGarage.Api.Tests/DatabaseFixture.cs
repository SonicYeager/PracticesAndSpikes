using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Tests;

[SetUpFixture]
public class DatabaseFixture
{
    private IContainer _container;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        _container = new ContainerBuilder()
            .WithImage("mariadb:latest")
            .WithEnvironment(new Dictionary<string, string>
            {
                {
                    "MARIADB_ROOT_PASSWORD", "my-secret"
                },
            })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(new Regex("mariadbd: ready for connections.")))
            .Build();

        await _container.StartAsync()
            .ConfigureAwait(false);

        var collection = new ServiceCollection();
        const string connectionString = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
        collection
            .AddPooledDbContextFactory<MyGarageDbContext>(
                static c => c.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        await using var provider = collection.BuildServiceProvider();
        var dbContextFactory = provider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.MigrateAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _container.StopAsync()
            .ConfigureAwait(false);
        await _container.DisposeAsync();
    }
}