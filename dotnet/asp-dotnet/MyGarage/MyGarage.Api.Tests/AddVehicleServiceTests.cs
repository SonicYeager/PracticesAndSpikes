using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.AddVehicle;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;
using MyGarage.Api.Tests.TestEntities;

namespace MyGarage.Api.Tests;

[TestFixture]
public sealed class AddVehicleServiceTests : DatabaseFixture
{
    /// <inheritdoc />
    protected override string DatabaseName
    {
        get => nameof(AddVehicleServiceTests);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task Add_WhenCalled_ShouldAddGarage()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var garage = TestGarageFactory.Create();
        dbContext.Set<Garage>().Add(garage);
        await dbContext.SaveChangesAsync();
        var service = new AddVehicleService(dbContext);
        var input = TestAddVehicleInputFactory.Create(garage.Id);

        // Act
        var vehicle = await service.Add(input);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(vehicle.Id, Is.GreaterThan(0));
            Assert.That(vehicle.Designation, Is.EqualTo(input.Designation));
            Assert.That(vehicle.Odometer, Is.EqualTo(input.Odometer));
            Assert.That(vehicle.FirstRegistration, Is.EqualTo(input.FirstRegistration));
            Assert.That(vehicle.LicensePlate, Is.EqualTo(input.LicensePlate));
            Assert.That(vehicle.FuelCapacity, Is.EqualTo(input.FuelCapacity));
            Assert.That(vehicle.PriceAtPurchase, Is.EqualTo(input.PriceAtPurchase));
        });
    }
}