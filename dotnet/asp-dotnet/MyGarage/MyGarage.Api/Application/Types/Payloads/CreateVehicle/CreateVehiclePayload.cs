namespace MyGarage.Api.Application.Types.Payloads.CreateVehicle;

public sealed record CreateVehiclePayload(Vehicle? Vehicle, IEnumerable<ICreateVehicleError> Errors);