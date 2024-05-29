namespace MyGarage.Api.Application.Types.Payloads;

public record CreateGaragePayload(Garage? Garage, IEnumerable<ICreateGarageError> Errors);