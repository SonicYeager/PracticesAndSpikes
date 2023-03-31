using DotPulsar;
using DotPulsar.Abstractions;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace PulsarWorker.Data;

public class JsonSchema<TMessage> : ISchema<TMessage>
{
    public TMessage? Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
    {
        return JsonSerializer.Deserialize<TMessage>(bytes.ToString());
    }
    public ReadOnlySequence<byte> Encode(TMessage message)
    {
        var converted = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(converted).ToSequence();
    }
    
    public SchemaInfo SchemaInfo { get; }
}