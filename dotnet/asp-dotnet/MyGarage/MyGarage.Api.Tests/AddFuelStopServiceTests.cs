using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyGarage.Api.Application.Services.AddFuelStop;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;
using MyGarage.Api.Tests.TestEntities;

namespace MyGarage.Api.Tests;

[TestFixture]
public sealed class AddFuelStopServiceTests : DatabaseFixture
{
    /// <inheritdoc />
    protected override string DatabaseName
    {
        get => nameof(AddFuelStopServiceTests);
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var dbContext = await GetMyGarageDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task Add_WhenCalled_ShouldAddFuelStop()
    {
        // Arrange
        var dbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<MyGarageDbContext>>();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var vehicle = TestVehicleFactory.Create();
        dbContext.Set<Vehicle>().Add(vehicle);
        await dbContext.SaveChangesAsync();
        var service = new AddFuelStopService(dbContext);
        var input = TestAddFuelStopInputFactory.Create(vehicle.Id);

        // Act
        var addedFuelStop = await service.Add(input);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(addedFuelStop.Id, Is.GreaterThan(0));
            Assert.That(addedFuelStop.VehicleId, Is.EqualTo(input.VehicleId));
            Assert.That(addedFuelStop.Date, Is.EqualTo(input.Date));
            Assert.That(addedFuelStop.AmountInLiters, Is.EqualTo(input.AmountInLiters));
            Assert.That(addedFuelStop.OdometerInKilometers, Is.EqualTo(input.OdometerInKilometers));
            Assert.That(addedFuelStop.TotalPriceInEuro, Is.EqualTo(input.TotalPriceInEuro));
            Assert.That(addedFuelStop.Note, Is.EqualTo(input.Note));
        });
    }
}