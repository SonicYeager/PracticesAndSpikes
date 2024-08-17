using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Services;
using MyGarage.Api.Application.Services.CreateGarage;
using MyGarage.Api.Persistence;

const string policyName = "AllowAllOrigins";

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

builder.Services.AddScoped<CreateVehicleService>();
builder.Services.AddScoped<CreateVehicleValidator>();
builder.Services.AddScoped<CreateGarageService>();
builder.Services.AddScoped<CreateGarageValidator>();

builder.Services.AddGraphQLServer()
    .AddTypes()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddMutationConventions()
    .AddDefaultTransactionScopeHandler()
    .RegisterDbContext<MyGarageDbContext>(DbContextKind.Pooled)
    .RegisterService<CreateVehicleService>()
    .RegisterService<CreateVehicleValidator>()
    .RegisterService<CreateGarageService>()
    .RegisterService<CreateGarageValidator>();

const string connectionString = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
builder.Services
    .AddPooledDbContextFactory<MyGarageDbContext>(
        static c => c.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddCors(static options =>
{
    options.AddPolicy(policyName, static builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors(policyName);
app.MapGraphQL();

await app.RunAsync();