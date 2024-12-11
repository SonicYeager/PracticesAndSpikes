using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Queries;

[QueryType]
public static class VehiclesQuery
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Vehicle> Vehicles(MyGarageDbContext dbContext)
    {
        return dbContext
            .Set<Vehicle>()
            .AsNoTracking()
            .AsSplitQuery()
            .OrderBy(static f => f.Id);
    }
}