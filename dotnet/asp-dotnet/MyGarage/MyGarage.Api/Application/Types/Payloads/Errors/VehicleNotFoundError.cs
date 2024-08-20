using MyGarage.Api.Application.Types.Payloads.AddFuelStop;

namespace MyGarage.Api.Application.Types.Payloads.Errors;

[ObjectType]
public sealed record VehicleNotFoundError(string Message) : DefaultError(Message), IAddFuelStopError;