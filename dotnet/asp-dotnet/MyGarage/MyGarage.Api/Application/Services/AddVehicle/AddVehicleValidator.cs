using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.AddVehicle;
using MyGarage.Api.Application.Types.Payloads.AddVehicle;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services.AddVehicle;

public sealed class AddVehicleValidator
{
    private readonly MyGarageDbContext _context;

    public AddVehicleValidator(MyGarageDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<IAddVehicleError>> Validate(AddVehicleInput input)
    {
        var errors = new List<IAddVehicleError>();
        if (_context.Set<Vehicle>().Any(g => g.Designation == input.Designation))
            errors.Add(new VehicleAlreadyExistsError("A vehicle with the same designation already exists."));

        var garage = await _context.Set<Garage>().FindAsync(input.GarageId);
        if (garage is null)
            errors.Add(new GarageNotFoundError("The garage does not exist."));

        return errors;
    }
}