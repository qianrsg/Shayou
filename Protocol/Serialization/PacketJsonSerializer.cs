using Shayou.Protocol.Messages;
using System.Text.Json;

namespace Shayou.Protocol.Serialization
{
    public static class PacketJsonSerializer
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static string Serialize(PacketEnvelope packet)
        {
            if (packet is PacketBatch batch)
            {
                return JsonSerializer.Serialize(CreateBatchPayload(batch), SerializerOptions);
            }

            return JsonSerializer.Serialize(packet, packet.GetType(), SerializerOptions);
        }

        public static PacketEnvelope Deserialize(string json)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;
            string kind = root.GetProperty("kind").GetString()
                ?? throw new InvalidOperationException("Packet kind is invalid.");

            return kind switch
            {
                PacketKinds.Batch => DeserializeBatch(root),
                PacketKinds.Command => DeserializePayload<CommandPacket>(json),
                PacketKinds.Response => DeserializePayload<ResponsePacket>(json),
                PacketKinds.Event => DeserializePayload<EventPacket>(json),
                PacketKinds.Snapshot => DeserializePayload<SnapshotPacket>(json),
                PacketKinds.Error => DeserializePayload<ErrorPacket>(json),
                _ => throw new InvalidOperationException($"Unknown packet kind: {kind}")
            };
        }

        private static TPacket DeserializePayload<TPacket>(string json)
            where TPacket : PacketEnvelope
        {
            return JsonSerializer.Deserialize<TPacket>(json, SerializerOptions)
                ?? throw new InvalidOperationException($"Packet JSON for {typeof(TPacket).Name} is invalid.");
        }

        private static PacketBatchWirePayload CreateBatchPayload(PacketBatch batch)
        {
            List<JsonElement> messages = new();

            foreach (PacketEnvelope message in batch.Messages)
            {
                string packetJson = Serialize(message);
                using JsonDocument messageDocument = JsonDocument.Parse(packetJson);
                messages.Add(messageDocument.RootElement.Clone());
            }

            return new PacketBatchWirePayload
            {
                Kind = batch.Kind,
                Key = batch.Key,
                Messages = messages
            };
        }

        private static PacketBatch DeserializeBatch(JsonElement root)
        {
            List<PacketEnvelope> messages = new();

            foreach (JsonElement message in root.GetProperty("messages").EnumerateArray())
            {
                messages.Add(Deserialize(message.GetRawText()));
            }

            return new PacketBatch
            {
                Kind = root.GetProperty("kind").GetString() ?? PacketKinds.Batch,
                Key = root.GetProperty("key").GetString() ?? PacketKinds.Batch,
                Messages = messages
            };
        }
    }

    public sealed record PacketBatchWirePayload
    {
        public string Kind { get; init; } = PacketKinds.Batch;

        public string Key { get; init; } = PacketKinds.Batch;

        public List<JsonElement> Messages { get; init; } = new();
    }
}
