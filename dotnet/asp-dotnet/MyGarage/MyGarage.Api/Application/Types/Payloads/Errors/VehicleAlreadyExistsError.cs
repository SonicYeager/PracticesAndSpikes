using MyGarage.Api.Application.Types.Payloads.AddVehicle;

namespace MyGarage.Api.Application.Types.Payloads.Errors;

[ObjectType]
public sealed record VehicleAlreadyExistsError(string Message) : DefaultError(Message), IAddVehicleError;