using MyGarage.Traffic.Application;

namespace MyGarage.Traffic;

public sealed class GetGaragesWorker : BackgroundService
{
    private readonly ILogger<GetGaragesWorker> _logger;
    private readonly IMyGarageService _service;

    public GetGaragesWorker(ILogger<GetGaragesWorker> logger, IMyGarageService service)
    {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(CreateGarageWorker), DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _service.GetGarages(stoppingToken);


            await foreach (var garage in result)
            {
                _logger.LogInformation("Garage: {Id} - {Designation} with {VehicleCount} vehicles", garage.Id, garage.Designation,
                    garage.Vehicles.Count);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}