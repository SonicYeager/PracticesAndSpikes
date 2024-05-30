namespace MyGarage.Api.Application.Types.Inputs.CreateVehicle;

public sealed record CreateVehicleInput(
    int GarageId,
    string Designation,
    string? LicensePlate,
    DateTimeOffset FirstRegistration,
    decimal Odometer,
    decimal PriceAtPurchase,
    decimal FuelCapacity);