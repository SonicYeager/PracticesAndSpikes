namespace MyGarage.Api.Application.Types.Payloads.AddFuelStop;

public sealed record AddFuelStopPayload(FuelStop? FuelStop, IEnumerable<IAddFuelStopError> Errors);