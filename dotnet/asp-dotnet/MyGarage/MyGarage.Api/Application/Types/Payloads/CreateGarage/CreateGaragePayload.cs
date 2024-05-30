namespace MyGarage.Api.Application.Types.Payloads.CreateGarage;

public sealed record CreateGaragePayload(Garage? Garage, IEnumerable<ICreateGarageError> Errors);