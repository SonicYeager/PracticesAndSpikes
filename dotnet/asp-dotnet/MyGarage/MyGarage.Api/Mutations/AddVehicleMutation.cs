using MyGarage.Api.Application.Services.AddVehicle;
using MyGarage.Api.Application.Types.Inputs.AddVehicle;
using MyGarage.Api.Application.Types.Payloads.AddVehicle;

namespace MyGarage.Api.Mutations;

[MutationType]
public static class AddVehicleMutation
{
    public static async Task<AddVehiclePayload?> AddVehicle(AddVehicleValidator validator, AddVehicleService service,
        AddVehicleInput input)
    {
        var errors = await validator.Validate(input);
        var vehicle = await service.Add(input);

        return new(vehicle, errors);
    }
}