using DotPulsar;
using DotPulsar.Abstractions;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PulsarWorker.Data
{
    public sealed class BaseMessage
    {
        public string MessageId { get; set; }
        public DateTime SendAt { get; set; }
    }
}
