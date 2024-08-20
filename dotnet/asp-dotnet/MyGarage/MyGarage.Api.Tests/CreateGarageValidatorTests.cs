using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.CreateGarage;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;
using MyGarage.Api.Tests.TestEntities;

namespace MyGarage.Api.Tests;

[TestFixture]
public sealed class CreateGarageValidatorTests : DatabaseFixture
{
    /// <inheritdoc />
    protected override string DatabaseName
    {
        get => nameof(CreateGarageValidatorTests);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task Validate_WhenCalledWithoutExistingGarage_ShouldNotReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var validator = new CreateGarageValidator(dbContext);

        // Act
        var errors = await validator.Validate(TestCreateGarageInputFactory.Create());

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public async Task Validate_WhenCalledWithExistingGarage_ShouldReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Set<Garage>().Add(TestGarageFactory.Create());
        await dbContext.SaveChangesAsync();
        var validator = new CreateGarageValidator(dbContext);

        // Act
        var errors = await validator.Validate(new("My Garage"));

        // Assert
        Assert.That(errors, Is.EquivalentTo(new List<GarageAlreadyExistsError>
        {
            new("A garage with the same designation already exists."),
        }));
    }
}