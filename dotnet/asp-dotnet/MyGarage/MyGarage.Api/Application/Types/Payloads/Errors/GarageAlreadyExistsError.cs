using MyGarage.Api.Application.Types.Payloads.CreateGarage;

namespace MyGarage.Api.Application.Types.Payloads.Errors;

[ObjectType]
public sealed record GarageAlreadyExistsError(string Message) : DefaultError(Message), ICreateGarageError;