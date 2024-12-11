using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Queries;

[QueryType]
public static class GaragesQuery
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Garage> Garages(MyGarageDbContext dbContext)
    {
        return dbContext
            .Set<Garage>()
            .AsNoTracking()
            .AsSplitQuery()
            .OrderBy(static g => g.Id);
    }
}