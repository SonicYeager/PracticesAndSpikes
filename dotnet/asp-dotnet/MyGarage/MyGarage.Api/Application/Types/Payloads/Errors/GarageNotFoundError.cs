using MyGarage.Api.Application.Types.Payloads.CreateVehicle;

namespace MyGarage.Api.Application.Types.Payloads.Errors;

[ObjectType]
public sealed record GarageNotFoundError(string Message) : DefaultError(Message), ICreateVehicleError;