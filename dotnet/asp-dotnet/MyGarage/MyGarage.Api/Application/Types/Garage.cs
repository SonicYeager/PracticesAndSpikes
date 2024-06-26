﻿namespace MyGarage.Api.Application.Types;

public sealed record Garage
{
    public int Id { get; init; }
    public required string Designation { get; init; } = string.Empty;

    public ICollection<Vehicle> Vehicles { get; init; } = new HashSet<Vehicle>();
}