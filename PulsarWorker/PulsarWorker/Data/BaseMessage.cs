using DotPulsar;
using DotPulsar.Abstractions;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PulsarWorker.Data;

public sealed record BaseMessage(string MessageId, DateTime SendAt);