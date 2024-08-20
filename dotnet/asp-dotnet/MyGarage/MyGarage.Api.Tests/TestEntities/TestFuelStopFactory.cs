using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestFuelStopFactory
{
    public static FuelStop Create(int vehicleId = 1) => new()
    {
        Date = DateTimeOffset.Now,
        VehicleId = vehicleId,
        AmountInLiters = 20,
        TotalPriceInEuro = 30,
        OdometerInKilometers = 1000,
    };
}