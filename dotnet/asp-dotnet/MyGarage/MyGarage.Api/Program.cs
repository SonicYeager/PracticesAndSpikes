using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Types.Payloads;
using MyGarage.Api.Persistence;

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();
builder.Services.AddGraphQLServer()
    .AddTypes()
    .AddType<GarageAlreadyExistsError>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddMutationConventions()
    .RegisterDbContext<MyGarageDbContext>(DbContextKind.Pooled);

const string connectionString = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
builder.Services
    .AddPooledDbContextFactory<MyGarageDbContext>(
        static c => c.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

app.MapGraphQL();

app.Run();