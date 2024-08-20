namespace MyGarage.Api.Application.Types.Inputs.AddVehicle;

public sealed record AddVehicleInput(
    int GarageId,
    string Designation,
    string? LicensePlate,
    DateTimeOffset FirstRegistration,
    decimal Odometer,
    decimal PriceAtPurchase,
    decimal FuelCapacity);