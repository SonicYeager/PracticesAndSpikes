using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.AddVehicle;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application.Services.AddVehicle;

public sealed class AddVehicleService
{
    private readonly MyGarageDbContext _context;

    public AddVehicleService(MyGarageDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle> Add(AddVehicleInput input)
    {
        var vehicle = new Vehicle
        {
            Designation = input.Designation,
            LicensePlate = input.LicensePlate,
            FirstRegistration = input.FirstRegistration,
            Odometer = input.Odometer,
            FuelCapacity = input.FuelCapacity,
            PriceAtPurchase = input.PriceAtPurchase,
        };

        _context.Set<Vehicle>().Add(vehicle);
        var garage = await _context.Set<Garage>().FindAsync(input.GarageId);
        garage!.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();

        return vehicle;
    }
}