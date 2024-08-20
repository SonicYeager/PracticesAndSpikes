using MyGarage.Traffic.Client;

namespace MyGarage.Traffic.Application;

public interface IMyGarageService
{
    public IAsyncEnumerable<IGetGarages_Garages_Edges_Node> GetGarages(CancellationToken stoppingToken);
    public IAsyncEnumerable<IGetVehicles_Vehicles_Edges_Node> GetVehicles(CancellationToken stoppingToken);
    public Task<ICreateGarage_CreateGarage_Garage?> CreateGarage(string designation, CancellationToken stoppingToken);
    public Task<IAddVehicle_AddVehicle_Vehicle?> AddVehicle(
        string designation,
        DateTimeOffset firstRegistration,
        string? licensePlate,
        decimal fuelCapacity,
        int garageId,
        decimal odometer,
        decimal priceAtPurchase,
        CancellationToken stoppingToken);
    public Task<IAddFuelStop_AddFuelStop_FuelStop?> AddFuelStop(
        int vehicleId,
        DateTimeOffset date,
        decimal amountInLiters,
        decimal odometerInKilometers,
        decimal totalPriceInEuro,
        string? note,
        CancellationToken stoppingToken);
}