using DotPulsar;
using DotPulsar.Extensions;
using DotPulsar.Internal.PulsarApi;
using PulsarWorker.Data;
using System.Buffers;
using System.Text;

namespace PulsarWorker.DotPulsarWorker;

public class ApachePulsarProducerWorker : BackgroundService
{
    private static async Task Produce()
    {
        await using var client = PulsarClient.Builder()
            .Build();

        await using var producer = client.NewProducer(new JsonSchema<BaseMessage>())
            .Topic("persistent://public/default/mytopic")
            .Create();

        for (var i = 0; i < Random.Shared.Next(10); i++)
        {
            var data = DataGenerator.Generate<BaseMessage>();
            await producer.Send(data);
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