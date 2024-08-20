namespace MyGarage.Api.Application.Types.Payloads.AddVehicle;

public sealed record AddVehiclePayload(Vehicle? Vehicle, IEnumerable<IAddVehicleError> Errors);