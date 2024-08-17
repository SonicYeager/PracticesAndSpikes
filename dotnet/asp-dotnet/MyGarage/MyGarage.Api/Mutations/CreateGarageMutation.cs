using MyGarage.Api.Application.Services.CreateGarage;
using MyGarage.Api.Application.Types.Inputs.CreateGarage;
using MyGarage.Api.Application.Types.Payloads.CreateGarage;

namespace MyGarage.Api.Mutations;

[MutationType]
public static class CreateGarageMutation
{
    public static async Task<CreateGaragePayload?> CreateGarage(CreateGarageValidator validator, CreateGarageService service,
        CreateGarageInput input)
    {
        var errors = await validator.Validate(input);
        var garage = await service.Create(input);

        return new(garage, errors);
    }
}