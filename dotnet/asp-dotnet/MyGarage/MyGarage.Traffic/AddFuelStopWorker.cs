using MyGarage.Traffic.Application;

namespace MyGarage.Traffic;

public sealed class AddFuelStopWorker : BackgroundService
{
    private readonly ILogger<AddFuelStopWorker> _logger;
    private readonly IMyGarageService _service;

    public AddFuelStopWorker(ILogger<AddFuelStopWorker> logger, IMyGarageService service)
    {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(AddFuelStopWorker), DateTimeOffset.Now);

        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var lastVehicle = await _service.GetVehicles(stoppingToken).LastAsync(stoppingToken);
            var result = await _service.AddFuelStop(
                lastVehicle.Id,
                DateTimeOffset.Now,
                50,
                10000,
                100,
                "Refueled with Jet Fuel",
                stoppingToken);

            if (result is not null)
                _logger.LogInformation("Successfully Created Vehicle: {Id}", result.Id);

            await Task.Delay(1000, stoppingToken);
        }
    }
}