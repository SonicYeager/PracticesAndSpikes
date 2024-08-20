using Microsoft.EntityFrameworkCore;
using MyGarage.Api.Application.Services.AddFuelStop;
using MyGarage.Api.Application.Services.AddVehicle;
using MyGarage.Api.Application.Services.CreateGarage;
using MyGarage.Api.Persistence;

const string policyName = "AllowAllOrigins";

var builder = WebApplication.CreateSlimBuilder(args);

builder.WebHost.UseKestrelHttpsConfiguration();

const string connectionString = "Server=localhost;Database=mygarage;user=root;password=my-secret;";
builder.Services
    .AddPooledDbContextFactory<MyGarageDbContext>(
        static c => c.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<AddVehicleService>();
builder.Services.AddScoped<AddVehicleValidator>();
builder.Services.AddScoped<CreateGarageService>();
builder.Services.AddScoped<CreateGarageValidator>();
builder.Services.AddScoped<AddFuelStopService>();
builder.Services.AddScoped<AddFuelStopValidator>();

builder.Services.AddGraphQLServer()
    .AddTypes()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .AddMutationConventions()
    .AddDefaultTransactionScopeHandler()
    .RegisterDbContext<MyGarageDbContext>(DbContextKind.Pooled)
    .RegisterService<AddVehicleService>()
    .RegisterService<AddVehicleValidator>()
    .RegisterService<CreateGarageService>()
    .RegisterService<CreateGarageValidator>()
    .RegisterService<AddFuelStopService>()
    .RegisterService<AddFuelStopValidator>();

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