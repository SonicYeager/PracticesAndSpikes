using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.AddFuelStop;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Payloads.AddFuelStop;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;
using MyGarage.Api.Tests.TestEntities;

namespace MyGarage.Api.Tests;

[TestFixture]
public sealed class AddFuelStopValidatorTests : DatabaseFixture
{
    /// <inheritdoc />
    protected override string DatabaseName
    {
        get => nameof(AddFuelStopValidatorTests);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task Validate_WhenCalledWithoutExistingVehicle_ShouldReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var input = TestAddFuelStopInputFactory.Create();
        var validator = new AddFuelStopValidator(dbContext);

        // Act
        var errors = await validator.Validate(TestAddFuelStopInputFactory.Create());

        // Assert
        Assert.That(errors, Is.EquivalentTo(new List<IAddFuelStopError>
        {
            new VehicleNotFoundError($"The vehicle with {input.VehicleId} does not exist."),
        }));
    }

    [Test]
    public async Task Validate_WhenCalledWithoutExistingVehicle_ShouldNotReturnError()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var vehicle = TestVehicleFactory.Create();
        dbContext.Set<Vehicle>().Add(vehicle);
        await dbContext.SaveChangesAsync();
        var validator = new AddFuelStopValidator(dbContext);

        // Act
        var errors = await validator.Validate(TestAddFuelStopInputFactory.Create(vehicle.Id));

        // Assert
        Assert.That(errors, Is.Empty);
    }
}