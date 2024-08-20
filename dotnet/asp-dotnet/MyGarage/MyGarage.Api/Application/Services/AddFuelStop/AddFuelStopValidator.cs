using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.AddFuelStop;
using MyGarage.Api.Application.Types.Payloads.AddFuelStop;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services.AddFuelStop;

public sealed class AddFuelStopValidator
{
    private readonly MyGarageDbContext _context;

    public AddFuelStopValidator(MyGarageDbContext context)
    {
        _context = context;
    }

    public Task<IEnumerable<IAddFuelStopError>> Validate(AddFuelStopInput input)
    {
        var errors = new List<IAddFuelStopError>();
        if (!_context.Set<Vehicle>().Any(g => g.Id == input.VehicleId))
            errors.Add(new VehicleNotFoundError($"The vehicle with {input.VehicleId} does not exist."));
        return Task.FromResult<IEnumerable<IAddFuelStopError>>(errors);
    }

    //TODO add validation to check wheter fuel stops date and odometer are plausible
    //for instance, the odometer should be higher than the previous fuel stop when the date is higher (the same date should be allowed)
    //if the date is smaller than the latest fuel stop, the odometer should be smaller than the latest fuel stop but higher than the previous one
}