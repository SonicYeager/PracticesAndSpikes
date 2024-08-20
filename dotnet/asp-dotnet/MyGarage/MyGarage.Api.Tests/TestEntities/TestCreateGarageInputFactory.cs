using MyGarage.Api.Application.Types.Inputs.CreateGarage;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestCreateGarageInputFactory
{
    public static CreateGarageInput Create(string designation = "My Garage") => new(designation);
}