using MyGarage.Api.Application.Services.AddFuelStop;
using MyGarage.Api.Application.Types.Inputs.AddFuelStop;
using MyGarage.Api.Application.Types.Payloads.AddFuelStop;

namespace MyGarage.Api.Mutations;

[MutationType]
public static class AddFuelStopMutation
{
    public static async Task<AddFuelStopPayload?> AddFuelStop(AddFuelStopValidator validator, AddFuelStopService service,
        AddFuelStopInput input)
    {
        var errors = (await validator.Validate(input)).ToList();
        if (errors.Count != 0)
            return new(null, errors);

        var vehicle = await service.Add(input);

        return new(vehicle, errors);
    }
}