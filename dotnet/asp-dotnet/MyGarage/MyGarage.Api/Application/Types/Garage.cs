namespace MyGarage.Api.Application.Types;

public sealed record Garage
{
    public required string Id { get; init; } = string.Empty;
    public required string Designation { get; init; } = string.Empty;
}