namespace MyGarage.App.Application;

public interface IMyGarageService
{
    public IAsyncEnumerable<IGetGarages_Garages_Edges_Node> GetGarages(CancellationToken stoppingToken);
    public IAsyncEnumerable<IGetVehicles_Vehicles_Edges_Node> GetVehicles(CancellationToken stoppingToken);
    public Task<ICreateGarage_CreateGarage_Garage?> CreateGarage(string designation, CancellationToken stoppingToken);
    public Task<ICreateVehicle_CreateVehicle_Vehicle?> CreateVehicle(
        string designation,
        DateTimeOffset firstRegistration,
        string? licensePlate,
        decimal fuelCapacity,
        int garageId,
        decimal odometer,
        decimal priceAtPurchase,
        CancellationToken stoppingToken);
}