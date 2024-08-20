using MyGarage.Api.Application.Types.Inputs.AddFuelStop;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestAddFuelStopInputFactory
{
    public static AddFuelStopInput Create(int vehicleId = 1) => new(
        vehicleId,
        20,
        30,
        1000,
        DateTimeOffset.Now,
        null
    );
}