using MyGarage.Api.Application.Types;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Application;

[MutationType]
public static class Mutation
{
    public static async Task<string> CreateGarage(MyGarageDbContext dbContext, string designation)
    {
        //TODO generate id
        var garage = new Garage
        {
            Id = "ID", Designation = designation,
        };
        dbContext.Set<Garage>().Add(garage);
        await dbContext.SaveChangesAsync();
        return "success";
    }
}