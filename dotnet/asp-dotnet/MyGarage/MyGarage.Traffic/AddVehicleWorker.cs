using MyGarage.Traffic.Application;

namespace MyGarage.Traffic;

public sealed class AddVehicleWorker : BackgroundService
{
    private readonly ILogger<AddVehicleWorker> _logger;
    private readonly IMyGarageService _service;

    public AddVehicleWorker(ILogger<AddVehicleWorker> logger, IMyGarageService service)
    {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(AddVehicleWorker), DateTimeOffset.Now);

        var id = 1;

        await Task.Delay(5000, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var firstGarage = await _service.GetGarages(stoppingToken).LastAsync(stoppingToken);
            var result = await _service.AddVehicle(
                $"Vehicle - {id}",
                DateTimeOffset.Now,
                null,
                50,
                firstGarage.Id,
                0,
                10000,
                stoppingToken);

            if (result is not null)
                _logger.LogInformation("Successfully Created Vehicle: {Id} - {Designation}",
                    result.Id,
                    result.Designation);

            await Task.Delay(100, stoppingToken);
            id++;
        }
    }
}