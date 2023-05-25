using Microsoft.Extensions.DependencyInjection;
using PulsarWorker.Data.AutoMapper;

namespace PulsarWorker.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguredAutoMapper(this IServiceCollection service)
    {
        return service.AddAutoMapper(typeof(AutoMapperConfig));
    }
}