using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.CreateGarage;
using MyGarage.Api.Application.Types.Payloads.CreateGarage;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services.CreateGarage;

public sealed class CreateGarageValidator
{
    private readonly MyGarageDbContext _context;

    public CreateGarageValidator(MyGarageDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<IEnumerable<ICreateGarageError>> Validate(CreateGarageInput input)
    {
        var errors = new List<ICreateGarageError>();

        if (_context.Set<Garage>().Any(g => g.Designation == input.Designation))
            errors.Add(new GarageAlreadyExistsError("A garage with the same designation already exists."));

        return errors;
    }
}