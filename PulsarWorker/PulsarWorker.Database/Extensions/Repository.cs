using Microsoft.EntityFrameworkCore;

namespace PulsarWorker.Database.Extensions;

public static class Repository
{
    public static TContext Connect<TContext>(DbContextOptions<TContext> options) where TContext : DbContext
    {
        return Activator.CreateInstance(typeof(TContext), options) as TContext ??
               throw new InvalidOperationException($"Unable to create context for {typeof(TContext).FullName})");
    }
}