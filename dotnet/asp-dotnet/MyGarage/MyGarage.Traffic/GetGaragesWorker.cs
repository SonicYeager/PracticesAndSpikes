using MyGarage.Traffic.Client;

namespace MyGarage.Traffic;

public sealed class GetGaragesWorker : BackgroundService
{
    private readonly ILogger<GetGaragesWorker> _logger;
    private readonly IMyGarageClient _client;

    public GetGaragesWorker(ILogger<GetGaragesWorker> logger, IMyGarageClient client)
    {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("GetGaragesWorker running at: {Time}", DateTimeOffset.Now);
            }

            var result = await _client.GetGarages.ExecuteAsync(stoppingToken);

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error Fetching Garages: {Message}", error.Message);
                }
            }
            else
            {
                foreach (var garage in result.Data!.Garages)
                {
                    _logger.LogInformation("Garage: {Id} - {Designation}", garage.Id, garage.Designation);
                }
            }
        }
    }
}