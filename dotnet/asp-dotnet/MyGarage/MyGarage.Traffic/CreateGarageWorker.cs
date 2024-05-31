using MyGarage.Traffic.Application;

namespace MyGarage.Traffic;

public sealed class CreateGarageWorker : BackgroundService
{
    private readonly ILogger<CreateGarageWorker> _logger;
    private readonly IMyGarageService _service;

    public CreateGarageWorker(ILogger<CreateGarageWorker> logger, IMyGarageService service)
    {
        _logger = logger;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation("{WorkerName} running at: {Time}", nameof(CreateGarageWorker), DateTimeOffset.Now);

        var id = 1;

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await _service.CreateGarage($"Worker Home - {id}", stoppingToken);


            if (result is not null)
                _logger.LogInformation("Successfully Created Garage: {Id} - {Designation}",
                    result.Id,
                    result.Designation);

            await Task.Delay(1000, stoppingToken);
            id++;
        }
    }
}