using MyGarage.Api.Application.Services.CreateVehicle;
using MyGarage.Api.Application.Types.Inputs.CreateVehicle;
using MyGarage.Api.Application.Types.Payloads.CreateVehicle;

namespace MyGarage.Api.Mutations;

[MutationType]
public static class CreateVehicleMutation
{
    public static async Task<CreateVehiclePayload?> CreateVehicle(CreateVehicleValidator validator, CreateVehicleService service,
        CreateVehicleInput input)
    {
        var errors = await validator.Validate(input);
        var vehicle = await service.Create(input);

        return new(vehicle, errors);
    }
}