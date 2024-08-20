namespace MyGarage.Api.Application.Types.Inputs.AddFuelStop;

public sealed record AddFuelStopInput(
    int VehicleId,
    decimal AmountInLiters,
    decimal TotalPriceInEuro,
    decimal OdometerInKilometers,
    DateTimeOffset Date,
    string? Note
);