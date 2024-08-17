using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.CreateGarage;
using MyGarage.Api.Application.Types.Inputs.CreateGarage;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Tests;

public sealed class CreateGarageValidatorTests
{
    private IServiceProvider ServiceProvider { get; set; }

    [SetUp]
    public void SetUp()
    {
        var collection = new ServiceCollection();
        const string connectionString = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
        collection
            .AddPooledDbContextFactory<MyGarageDbContext>(
                static c => c.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        ServiceProvider = collection.BuildServiceProvider();
    }

    [Test]
    public async Task Validate_WhenCalledWithoutExistingGarage_ShouldReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var validator = new CreateGarageValidator(dbContext);

        // Act
        var errors = await validator.Validate(new CreateGarageInput("My Garage"));

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [TearDown]
    public void TearDown()
    {
        if (ServiceProvider is IDisposable disposable)
            disposable.Dispose();
    }
}