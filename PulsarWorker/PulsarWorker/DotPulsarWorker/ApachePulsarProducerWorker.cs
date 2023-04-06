using System.Text;
using System.Text.Json;
using DotPulsar;
using DotPulsar.Extensions;
using PulsarWorker.Data;
using PulsarWorker.Data.PulsarMessages;
using BaseMessage = PulsarWorker.Data.BaseMessage;

namespace PulsarWorker.DotPulsarWorker;

public class ApachePulsarProducerWorker : BackgroundService
{
    private static async Task Produce()
    {
        await using var client = PulsarClient.Builder()
            .Build();

        await using var producer = client.NewProducer(Schema.String)
            .Topic("persistent://public/default/mytopic")
            .Create();

        for (var i = 0; i < Random.Shared.Next(10); i++)
        {
            var data = DataGenerator.Generate<BaseMessage>();
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