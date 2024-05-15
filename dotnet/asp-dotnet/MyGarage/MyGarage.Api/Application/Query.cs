using MyGarage.Api.Application.Types;

namespace MyGarage.Api.Application;

[QueryType]
public static class Query
{
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Garage> Garages()
    {
        return new List<Garage>
        {
            new()
            {
                Id = "2024_1", Designation = "Car",
            },
            new()
            {
                Id = "2024_2", Designation = "Motorcycle",
            },
            new()
            {
                Id = "2024_3", Designation = "Bicycle",
            },
        }.AsQueryable();
    }
}