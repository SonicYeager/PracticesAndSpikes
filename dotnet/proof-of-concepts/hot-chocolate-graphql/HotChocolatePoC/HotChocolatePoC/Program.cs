using HotChocolatePoC.AutoMapperConfig;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.MutationTypes;
using HotChocolatePoC.QueryTypes;
using HotChocolatePoC.Subscriptions;
using HotChocolatePoC.TypeExtensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddPooledDbContextFactory<ArticlesDbContext>(options =>
{
    options.UseMySql(connectionString, GetServerVersion(connectionString));
});

builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperConfiguration));
builder.Services.AddTransient<CustomsTariffRateResolver>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddGraphQLServer()
    .RegisterDbContextFactory<ArticlesDbContext>()
    .AddMutationConventions()
    .AddSubscriptionType<ArticleAddedSubscription>()
    .AddTypeExtension<ArticleExtension>()
    .AddAuthorization()
    .AddMutationType<ArticleMutation>()
    .AddQueryType<ArticleQuery>()
    .AddInMemorySubscriptions()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddErrorFilter(error =>
    {
        if (error.Exception is System.Collections.Generic.KeyNotFoundException ex)
        {
            return ErrorBuilder
                .FromError(error)
                .SetMessage(ex.Message)
                .SetCode("KEY_NOT_FOUND")
                .Build();
        }

        return error;
    })
    .ModifyRequestOptions(options =>
        options.IncludeExceptionDetails = builder.Environment.IsDevelopment());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWebSockets();

app.MapGraphQL();

app.Run();

static ServerVersion GetServerVersion(string? connectionString)
{
    try
    {
        // Requires a reachable database; fails fast otherwise (see fallback below).
        return ServerVersion.AutoDetect(connectionString);
    }
    catch (Exception ex)
    {
        // Startup (and tests) must not require a live database just to resolve
        // the server version — SQL-compat details only. Fall back to recent MariaDB.
        Console.WriteLine($"Warning: DB version auto-detect failed ({ex.Message}); falling back to MariaDB 11.7.");
        return new MariaDbServerVersion(new Version(11, 7, 0));
    }
}
