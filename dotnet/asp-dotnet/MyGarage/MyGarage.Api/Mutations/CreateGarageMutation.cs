using MyGarage.Api.Application.Types;
using MyGarage.Api.Application.Types.Inputs.CreateGarage;
using MyGarage.Api.Application.Types.Payloads.CreateGarage;
using MyGarage.Api.Application.Types.Payloads.Errors;
using MyGarage.Api.Persistence;

namespace MyGarage.Api.Mutations;

[MutationType]
public static class CreateGarageMutation
{
    public static async Task<CreateGaragePayload?> CreateGarage(MyGarageDbContext dbContext, CreateGarageInput input)
    {
        if (dbContext.Set<Garage>().Any(g => g.Designation == input.Designation))
        {
            return new(null, new ICreateGarageError[]
            {
                new GarageAlreadyExistsError("A garage with the same designation already exists."),
            });
        }

        var garage = new Garage
        {
            Designation = input.Designation,
        };
        dbContext.Set<Garage>().Add(garage);
        await dbContext.SaveChangesAsync();

        return new(garage, []);
    }
}