using MyGarage.Traffic.Application;

namespace MyGarage.Traffic;

public sealed class CreateVehicleWorker : BackgroundService
{
    private readonly ILogger<CreateVehicleWorker> _logger;
    private readonly IMyGarageService _service;

    public CreateVehicleWorker(ILogger<CreateVehicleWorker> logger, IMyGarageService service)
    {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(CreateVehicleWorker), DateTimeOffset.Now);

        var id = 1;

        while (!stoppingToken.IsCancellationRequested)
        {
            var firstGarage = await _service.GetGarages(stoppingToken).LastAsync(stoppingToken);
            var result = await _service.CreateVehicle(
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

            await Task.Delay(1000, stoppingToken);
            id++;
        }
    }
}