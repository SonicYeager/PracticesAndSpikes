using System.Runtime.CompilerServices;

namespace MyGarage.App.Application;

public sealed class MyGarageService : IMyGarageService
{
    private readonly IMyGarageClient _myGarageClient;
    private readonly ILogger<MyGarageService> _logger;

    public MyGarageService(IMyGarageClient myGarageClient, ILogger<MyGarageService> logger)
    {
        _myGarageClient = myGarageClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IGetGarages_Garages_Edges_Node> GetGarages([EnumeratorCancellation] CancellationToken stoppingToken)
    {
        var result = await _myGarageClient.GetGarages.ExecuteAsync(10, null, stoppingToken);

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error Fetching Garages: {Message}", error.Message);
            }
        }
        else
        {
            if (result.Data!.Garages!.Edges!.Any())
                foreach (var garage in result.Data!.Garages!.Edges!.Select(static e => e.Node))
                {
                    yield return garage;
                }

            while (result.Data!.Garages!.PageInfo.HasNextPage)
            {
                foreach (var garage in result.Data!.Garages!.Edges!.Select(static e => e.Node))
                {
                    yield return garage;
                }

                result = await _myGarageClient.GetGarages.ExecuteAsync(10,
                    result.Data!.Garages!.Edges![result.Data!.Garages!.Edges.Count - 1].Cursor, stoppingToken);
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<IGetVehicles_Vehicles_Edges_Node> GetVehicles([EnumeratorCancellation] CancellationToken stoppingToken)
    {
        var result = await _myGarageClient.GetVehicles.ExecuteAsync(10, null, stoppingToken);

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error Fetching Garages: {Message}", error.Message);
            }
        }
        else
        {
            if (result.Data!.Vehicles!.Edges!.Any())
                foreach (var vehicle in result.Data!.Vehicles!.Edges!.Select(static e => e.Node))
                {
                    yield return vehicle;
                }

            while (result.Data!.Vehicles!.PageInfo.HasNextPage)
            {
                foreach (var vehicle in result.Data!.Vehicles!.Edges!.Select(static e => e.Node))
                {
                    yield return vehicle;
                }

                result = await _myGarageClient.GetVehicles.ExecuteAsync(10,
                    result.Data!.Vehicles!.Edges![result.Data!.Vehicles!.Edges.Count - 1].Cursor, stoppingToken);
            }
        }
    }

    /// <inheritdoc />
    public async Task<ICreateGarage_CreateGarage_Garage?> CreateGarage(string designation, CancellationToken stoppingToken)
    {
        var result = await _myGarageClient.CreateGarage.ExecuteAsync(designation, stoppingToken);

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error Creating Garage: {Message}", error.Message);
            }
        }
        else
        {
            if (result.Data!.CreateGarage!.Errors.Any())
                foreach (var error in result.Data.CreateGarage.Errors)
                {
                    if (error is CreateGarage_CreateGarage_Errors_GarageAlreadyExistsError alreadyExistsError)
                        _logger.LogError("{Message}", alreadyExistsError.Message);
                }
            else
                return result.Data!.CreateGarage!.Garage!;
        }

        return default;
    }

    /// <inheritdoc />
    public async Task<ICreateVehicle_CreateVehicle_Vehicle?> CreateVehicle(
        string designation,
        DateTimeOffset firstRegistration,
        string? licensePlate,
        decimal fuelCapacity,
        int garageId,
        decimal odometer,
        decimal priceAtPurchase,
        CancellationToken stoppingToken)
    {
        var result = await _myGarageClient.CreateVehicle.ExecuteAsync(
            designation, firstRegistration, licensePlate,
            fuelCapacity, garageId, odometer, priceAtPurchase, stoppingToken);

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError("Error Creating Garage: {Message}", error.Message);
            }
        }
        else
        {
            if (result.Data!.CreateVehicle!.Errors.Any())
                foreach (var error in result.Data.CreateVehicle.Errors)
                {
                    switch (error)
                    {
                        case CreateVehicle_CreateVehicle_Errors_VehicleAlreadyExistsError alreadyExistsError:
                            _logger.LogError("{Message}", alreadyExistsError.Message);
                            break;
                        case CreateVehicle_CreateVehicle_Errors_GarageNotFoundError notFoundError:
                            _logger.LogError("{Message}", notFoundError.Message);
                            break;
                    }
                }
            else
                return result.Data!.CreateVehicle!.Vehicle!;
        }

        return default;
    }
}