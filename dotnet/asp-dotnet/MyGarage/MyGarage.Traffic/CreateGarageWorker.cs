using MyGarage.Traffic.Client;

namespace MyGarage.Traffic;

public sealed class CreateGarageWorker : BackgroundService
{
    private readonly ILogger<CreateGarageWorker> _logger;
    private readonly IMyGarageClient _client;

    public CreateGarageWorker(ILogger<CreateGarageWorker> logger, IMyGarageClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(CreateGarageWorker), DateTimeOffset.Now);
        }

        var id = 1;

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await _client.CreateGarage.ExecuteAsync(new()
            {
                Designation = $"Worker Home - {id}",
            }, stoppingToken);

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error Creating Garage: {Message}", error.Message);
                }
            }
            else
            {
                if (result.Data!.CreateGarage!.Errors.Any())
                {
                    foreach (var error in result.Data.CreateGarage.Errors)
                    {
                        if (error is CreateGarage_CreateGarage_Errors_GarageAlreadyExistsError alreadyExistsError)
                            _logger.LogError("{Message}", alreadyExistsError.Message);
                    }
                }
                else
                {
                    _logger.LogInformation("Successfully Created Garage: {Id} - {Designation}",
                        result.Data!.CreateGarage!.Garage!.Id,
                        result.Data!.CreateGarage!.Garage!.Designation);
                }
            }

            await Task.Delay(1000, stoppingToken);
            id++;
        }
    }
}