using MyGarage.Api.Application.Types.Inputs.CreateVehicle;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestCreateVehicleInputFactory
{
    public static CreateVehicleInput Create(int garageId = 1, string designation = "My Vehicle") => new(
        garageId,
        designation,
        "ABC-123",
        DateTimeOffset.Now,
        1000,
        10000,
        50
    );
}