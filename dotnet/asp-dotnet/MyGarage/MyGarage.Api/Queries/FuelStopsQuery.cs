using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Queries;

[QueryType]
public static class FuelStopsQuery
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<FuelStop> FuelStops(MyGarageDbContext dbContext)
    {
        return dbContext.Set<FuelStop>().AsNoTracking();
    }
}