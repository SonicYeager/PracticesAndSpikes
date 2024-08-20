using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Tests.TestEntities;

public static class TestGarageFactory
{
    public static Garage Create(string designation = "My Garage") => new()
    {
        Designation = designation,
    };
}