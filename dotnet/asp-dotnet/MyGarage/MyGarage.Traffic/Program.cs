using MyGarage.Traffic;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<GetGaragesWorker>();

builder.Services.AddMyGarageClient()
    .ConfigureHttpClient(static client => client.BaseAddress = new("https://localhost:5001/graphql"));

var host = builder.Build();
host.Run();

// To Regenerate the client, run the following command:
// dotnet graphql init https://localhost:5001/graphql/ -n MyGarageClient

// Just in Case its not yet available, install the StrawberryShake.Tools:
// dotnet new tool-manifest
// dotnet tool install StrawberryShake.Tools --local
