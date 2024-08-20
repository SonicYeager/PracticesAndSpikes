namespace MyGarage.Api.Application.Types.Payloads.AddFuelStop;

public sealed record AddFuelStopPayload(FuelStop? Vehicle, IEnumerable<IAddFuelStopError> Errors);