using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.CreateGarage;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services.CreateGarage;

public sealed class CreateGarageService
{
    private readonly MyGarageDbContext _context;

    public CreateGarageService(MyGarageDbContext context)
    {
        _context = context;
    }

    public async Task<Garage> Create(CreateGarageInput input)
    {
        var garage = new Garage
        {
            Designation = input.Designation,
        };

        _context.Set<Garage>().Add(garage);
        await _context.SaveChangesAsync();

        return garage;
    }
}