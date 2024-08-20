using MyGarage.Api.Application.Types.Inputs.AddVehicle;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestAddVehicleInputFactory
{
    public static AddVehicleInput Create(int garageId = 1, string designation = "My Vehicle") => new(
        garageId,
        designation,
        "ABC-123",
        DateTimeOffset.Now,
        1000,
        10000,
        50
    );
}