namespace Shayou.Protocol.Messages
{
    public record ResponsePacket : PacketEnvelope
    {
        public ResponsePacket()
        {
            Kind = PacketKinds.Response;
        }

        public bool Success { get; init; }

        public string? ErrorMessage { get; init; }
    }
}
