using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.CreateVehicle;
using MyGarage.Api.Application.Types.Payloads.CreateVehicle;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Mutations;

[MutationType]
public static class CreateVehicleMutation
{
    public static async Task<CreateVehiclePayload?> CreateVehicle(MyGarageDbContext dbContext, CreateVehicleInput input)
    {
        if (dbContext.Set<Vehicle>().Any(g => g.Designation == input.Designation))
        {
            return new(null, new ICreateVehicleError[]
            {
                new VehicleAlreadyExistsError("A vehicle with the same designation already exists."),
            });
        }

        var garage = await dbContext.Set<Garage>().FindAsync(input.GarageId);
        if (garage is null)
        {
            return new(null, new ICreateVehicleError[]
            {
                new GarageNotFoundError("The garage does not exist."),
            });
        }

        var vehicle = new Vehicle
        {
            Designation = input.Designation,
            LicensePlate = input.LicensePlate,
            FirstRegistration = input.FirstRegistration,
            Odometer = input.Odometer,
            FuelCapacity = input.FuelCapacity,
            PriceAtPurchase = input.PriceAtPurchase,
        };
        dbContext.Set<Vehicle>().Add(vehicle);
        garage.Vehicles.Add(vehicle);
        await dbContext.SaveChangesAsync();

        return new(vehicle, []);
    }
}