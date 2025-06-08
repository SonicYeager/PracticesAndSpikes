using FindACoach.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Hardcoded ConnectionString für MariaDB
const string connectionString = "Server=localhost;Port=3306;Database=FindACoach;User=root;Password=my-secret;";

builder.Services.AddMySql<FindACoachDbContext>(connectionString, ServerVersion.AutoDetect(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();