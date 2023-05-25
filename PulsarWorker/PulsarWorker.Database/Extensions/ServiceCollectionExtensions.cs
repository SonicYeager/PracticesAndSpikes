using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Database.Context;

namespace PulsarWorker.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection service)
    {
        return service.AddDbContext<DatabaseContext>();
    }
}