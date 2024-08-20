using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestVehicleFactory
{
    public static Vehicle Create(string designation = "My Vehicle") => new()
    {
        Designation = designation,
        LicensePlate = "ABC-123",
        FirstRegistration = DateTimeOffset.Now,
        Odometer = 1000,
        PriceAtPurchase = 10000,
        FuelCapacity = 50,
    };
}