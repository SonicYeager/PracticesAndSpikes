using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.CreateGarage;
using MyGarage.Api.Persistence;
using MyGarage.Api.Tests.TestEntities;

namespace MyGarage.Api.Tests;

[TestFixture]
public sealed class CreateGarageServiceTests : DatabaseFixture
{
    /// <inheritdoc />
    protected override string DatabaseName
    {
        get => nameof(CreateGarageServiceTests);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task Create_WhenCalled_ShouldAddGarage()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var service = new CreateGarageService(dbContext);
        var input = TestCreateGarageInputFactory.Create();

        // Act
        var garage = await service.Create(input);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(garage.Id, Is.GreaterThan(0));
            Assert.That(garage.Designation, Is.EqualTo(input.Designation));
            Assert.That(garage.Vehicles, Is.Empty);
        });
    }
}