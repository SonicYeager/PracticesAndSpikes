using Amazon.Runtime;
using Amazon.S3;
using FileTesting.Data;
using FileTesting.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for HTTP/2 and HTTP/3
builder.WebHost.ConfigureKestrel(static options =>
{
    // HTTP endpoint: HTTP/1.1 and HTTP/2 (H2C)
    // Note: HTTP/3 always requires TLS and won't work here.
    options.ListenLocalhost(5000, static o => o.Protocols = HttpProtocols.Http1AndHttp2);

    // HTTPS endpoint: HTTP/1.1, HTTP/2, and HTTP/3
    options.ListenLocalhost(5001, static o =>
    {
        o.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        o.UseHttps();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DB Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Use fixed version to avoid connection during 'migrations add' if DB is offline.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(10, 6, 0))));

// AWS / MinIO
// Manual AWS S3 Config for MinIO
// ForcePathStyle is critical for MinIO
// Config matches appsettings structure

builder.Services.AddSingleton<IAmazonS3>(_ =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = builder.Configuration["AWS:ServiceURL"],
        ForcePathStyle = true, // REQUIRED for MinIO
        UseHttp = true,
    };
    var creds = new BasicAWSCredentials(
        builder.Configuration["AWS:AccessKey"],
        builder.Configuration["AWS:SecretKey"]
    );
    return new AmazonS3Client(creds, config);
});

builder.Services.AddScoped<IMinioClient, MinioService>();
builder.Services.AddScoped<IScalingService, ScalingService>();
builder.Services.AddScoped<IImageRepairService, ImageRepairService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();