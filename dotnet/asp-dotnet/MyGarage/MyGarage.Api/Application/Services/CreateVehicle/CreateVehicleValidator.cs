using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.CreateVehicle;
using MyGarage.Api.Application.Types.Payloads.CreateVehicle;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services;

public sealed class CreateVehicleValidator
{
    private readonly MyGarageDbContext _context;

    public CreateVehicleValidator(MyGarageDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ICreateVehicleError>> Validate(CreateVehicleInput input)
    {
        var errors = new List<ICreateVehicleError>();
        if (_context.Set<Vehicle>().Any(g => g.Designation == input.Designation))
            errors.Add(new VehicleAlreadyExistsError("A vehicle with the same designation already exists."));

        var garage = await _context.Set<Garage>().FindAsync(input.GarageId);
        if (garage is null)
            errors.Add(new GarageNotFoundError("The garage does not exist."));

        return errors;
    }
}