namespace MyGarage.Api.Application.Types;

public sealed record FuelStop
{
    public int Id { get; init; }
    public int VehicleId { get; init; }
    public decimal AmountInLiters { get; init; }
    public decimal TotalPriceInEuro { get; init; }
    public decimal OdometerInKilometers { get; init; }
    public DateTimeOffset Date { get; init; }
    public string? Note { get; init; }
};