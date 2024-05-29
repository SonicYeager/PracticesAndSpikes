using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application;

[QueryType]
public static class Query
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Garage> Garages(MyGarageDbContext dbContext)
    {
        return dbContext.Set<Garage>().AsNoTracking();
    }
}