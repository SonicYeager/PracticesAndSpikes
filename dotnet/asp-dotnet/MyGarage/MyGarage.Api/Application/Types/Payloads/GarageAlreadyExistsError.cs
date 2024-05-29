namespace MyGarage.Api.Application.Types.Payloads;

public sealed record GarageAlreadyExistsError(string Message) : DefaultError(Message), ICreateGarageError;