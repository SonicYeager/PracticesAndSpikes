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
        get => $"server=localhost;database={DatabaseName};username=root;password=my-secret;port=5432;";
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
            .WithImage("postgres:latest")
            .WithEnvironment(new Dictionary<string, string>
            {
                {
                    "POSTGRES_PASSWORD", "my-secret"
                },
                {
                    "POSTGRES_USER", "root"
                },
            })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(ReadyLogRegex()))
            .Build();

        await _container.StartAsync();

        var collection = new ServiceCollection();
        collection
            .AddPooledDbContextFactory<MyGarageDbContext>(
                c => c.UseNpgsql(ConnectionString));

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

    [GeneratedRegex("PostgreSQL init process complete; ready for start up.")]
    private static partial Regex ReadyLogRegex();
}