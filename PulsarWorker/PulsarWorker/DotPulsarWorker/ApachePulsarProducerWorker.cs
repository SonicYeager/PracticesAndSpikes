using System.Text.Json;
using DotPulsar;
using DotPulsar.Extensions;
using BaseMessage = PulsarWorker.Data.BaseMessage;

namespace PulsarWorker.DotPulsarWorker;

public sealed class ApachePulsarProducerWorker : BackgroundService
{
    private static async Task Produce()
    {
        await using var client = PulsarClient.Builder().ServiceUrl(new("pulsar://pulsar:6650"))
            .Build();

        await using var producer = client.NewProducer(Schema.String)
            .Topic("persistent://public/default/mytopic")
            .Create();

        for (var i = 0; i < 10; i++)
        {
            var data = new BaseMessage(Guid.NewGuid().ToString(), DateTime.Now);
            var encoded = JsonSerializer.Serialize(data);

            await producer.Send(encoded);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Produce();
            await Task.Delay(Random.Shared.Next(100, 1000), stoppingToken);
        }
    }
}