using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Tests;

[SetUpFixture]
public partial class DatabaseFixture
{
    private string ConnectionString
    {
        get => $"Server=localhost;Database={DatabaseName};user=root;password=my-secret;";
    }

    protected virtual string DatabaseName
    {
        get => "mygarage";
    }

    private IContainer _container;

    protected IServiceProvider ServiceProvider { get; private set; }

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
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(MariaDbReadyLogRegex()))
            .Build();

        await _container.StartAsync();

        var collection = new ServiceCollection();
        collection
            .AddPooledDbContextFactory<MyGarageDbContext>(
                c => c.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString)));

        ServiceProvider = collection.BuildServiceProvider();

        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    protected async Task<MyGarageDbContext> GetMyGarageDbContext()
    {
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        return await dbContextFactory.CreateDbContextAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (ServiceProvider is IAsyncDisposable asyncDisposable) await asyncDisposable.DisposeAsync();
        else if (ServiceProvider is IDisposable disposable) disposable.Dispose();

        await _container.StopAsync()
            .ConfigureAwait(false);
        await _container.DisposeAsync();
    }

    [GeneratedRegex("mariadbd: ready for connections.")]
    private static partial Regex MariaDbReadyLogRegex();
}