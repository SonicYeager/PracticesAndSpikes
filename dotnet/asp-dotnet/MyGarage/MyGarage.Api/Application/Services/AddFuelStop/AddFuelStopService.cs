using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.AddFuelStop;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services.AddFuelStop;

public sealed class AddFuelStopService
{
    private readonly MyGarageDbContext _context;

    public AddFuelStopService(MyGarageDbContext context)
    {
        _context = context;
    }

    public async Task<FuelStop> Add(AddFuelStopInput input)
    {
        var fuelStop = new FuelStop
        {
            Date = input.Date,
            OdometerInKilometers = input.OdometerInKilometers,
            AmountInLiters = input.AmountInLiters,
            TotalPriceInEuro = input.TotalPriceInEuro,
            Note = input.Note,
            VehicleId = input.VehicleId,
        };

        _context.Set<FuelStop>().Add(fuelStop);
        var vehicle = await _context.Set<Vehicle>().FindAsync(input.VehicleId);
        vehicle!.FuelStops.Add(fuelStop);
        await _context.SaveChangesAsync();

        return fuelStop;
    }
}