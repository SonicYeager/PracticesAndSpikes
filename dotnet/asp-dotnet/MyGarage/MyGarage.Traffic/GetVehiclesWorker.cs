using MyGarage.Traffic.Application;

namespace MyGarage.Traffic;

public sealed class GetVehiclesWorker : BackgroundService
{
    private readonly ILogger<GetVehiclesWorker> _logger;
    private readonly IMyGarageService _service;

    public GetVehiclesWorker(ILogger<GetVehiclesWorker> logger, IMyGarageService service)
    {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(GetVehiclesWorker), DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = _service.GetVehicles(stoppingToken);


            await foreach (var vehicle in result)
            {
                _logger.LogInformation("Garage: {Id} - {Designation}", vehicle.Id, vehicle.Designation);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}