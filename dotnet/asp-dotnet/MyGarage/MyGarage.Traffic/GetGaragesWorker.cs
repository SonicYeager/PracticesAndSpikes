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
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(CreateGarageWorker), DateTimeOffset.Now);
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await _client.GetGarages.ExecuteAsync(10, null, stoppingToken);

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error Fetching Garages: {Message}", error.Message);
                }
            }
            else
            {
                if (result.Data!.Garages!.Edges!.Any())
                {
                    foreach (var garage in result.Data!.Garages!.Edges!.Select(static e => e.Node))
                    {
                        _logger.LogInformation("Garage: {Id} - {Designation}", garage.Id, garage.Designation);
                    }
                }

                while (result.Data!.Garages!.PageInfo.HasNextPage)
                {
                    foreach (var garage in result.Data!.Garages!.Edges!.Select(static e => e.Node))
                    {
                        _logger.LogInformation("Garage: {Id} - {Designation}", garage.Id, garage.Designation);
                    }

                    result = await _client.GetGarages.ExecuteAsync(10,
                        result.Data!.Garages!.Edges![result.Data!.Garages!.Edges.Count - 1].Cursor, stoppingToken);
                }
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}