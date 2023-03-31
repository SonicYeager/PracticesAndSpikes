using DotPulsar;
using DotPulsar.Abstractions;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PulsarWorker.Data
{
    public record BaseMessage(string MessageId, DateTime SendAt) : ISchema<BaseMessage>
    {
        public BaseMessage Decode(ReadOnlySequence<byte> bytes, byte[]? schemaVersion = null)
        {
            return JsonSerializer.Deserialize<BaseMessage>(bytes.ToString());
        }
        public ReadOnlySequence<byte> Encode(BaseMessage message)
        {
            var converted = JsonSerializer.Serialize(message);
            return Encoding.UTF8.GetBytes(converted).ToSequence();
        }
        public SchemaInfo SchemaInfo { get; }
    }
}
