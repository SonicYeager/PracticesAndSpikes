using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.CreateVehicle;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Payloads.CreateVehicle;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;
using MyGarage.Api.Tests.TestEntities;

namespace MyGarage.Api.Tests;

[TestFixture]
public sealed class CreateVehicleValidatorTests : DatabaseFixture
{
    /// <inheritdoc />
    protected override string DatabaseName
    {
        get => nameof(CreateVehicleValidatorTests);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task Validate_WhenCalledWithoutExistingGarage_ShouldReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var validator = new CreateVehicleValidator(dbContext);

        // Act
        var errors = await validator.Validate(TestCreateVehicleInputFactory.Create());

        // Assert
        Assert.That(errors, Is.EquivalentTo(new List<ICreateVehicleError>
        {
            new GarageNotFoundError("The garage does not exist."),
        }));
    }

    [Test]
    public async Task Validate_WhenCalledWithoutExistingVehicle_ShouldNotReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var garage = TestGarageFactory.Create();
        dbContext.Set<Garage>().Add(garage);
        await dbContext.SaveChangesAsync();
        var validator = new CreateVehicleValidator(dbContext);

        // Act
        var errors = await validator.Validate(TestCreateVehicleInputFactory.Create(garage.Id));

        // Assert
        Assert.That(errors, Is.Empty);
    }

    [Test]
    public async Task Validate_WhenCalledWithExistingVehicle_ShouldReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Set<Vehicle>().Add(TestVehicleFactory.Create());
        var garage = TestGarageFactory.Create();
        dbContext.Set<Garage>().Add(garage);
        await dbContext.SaveChangesAsync();
        var validator = new CreateVehicleValidator(dbContext);

        // Act
        var errors = await validator.Validate(TestCreateVehicleInputFactory.Create(garage.Id));

        // Assert
        Assert.That(errors, Is.EquivalentTo(new List<ICreateVehicleError>
        {
            new VehicleAlreadyExistsError("A vehicle with the same designation already exists."),
        }));
    }
}