using HotChocolatePoC.AutoMapperConfig;
using HotChocolatePoC.Database.Context;
using HotChocolatePoC.MutationTypes;
using HotChocolatePoC.QueryTypes;
using HotChocolatePoC.TypeExtensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<DbContext, ArticlesDbContext>(options =>
{
    var version = ServerVersion.AutoDetect(connectionString);
    options.UseMySql(connectionString, version);
});

builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));
builder.Services.AddTransient<CustomsTariffRateResolver>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<ArticlesDbContext>()
    .AddDefaultTransactionScopeHandler()
    .AddMutationConventions()
    //.AddMutationConventions(applyToAllMutations: true)
    //.AddSubscriptionType<ArticleAddedSubscription>() //Web socket based
    .AddTypeExtension<ArticleExtension>()
    .AddAuthorization()
    .AddMutationType<ArticleMutation>()
    .AddQueryType<ArticleQuery>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

builder.Services.AddInMemorySubscriptions();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseWebSockets();

app.MapGraphQL();

app.Run();