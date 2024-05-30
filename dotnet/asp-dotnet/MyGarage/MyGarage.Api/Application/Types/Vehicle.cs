namespace MyGarage.Api.Application.Types;

public sealed record Vehicle
{
    public int Id { get; init; }
    public string Designation { get; init; } = null!;
    public string? LicensePlate { get; init; }

    public DateTimeOffset FirstRegistration { get; init; }

    public decimal Odometer { get; init; }
    public decimal PriceAtPurchase { get; init; }
    public decimal FuelCapacity { get; init; }

    // TODO Services (repairs, maintenance and similar -> done by a workshop)
    // TODO FuelStops (refuelings)
}