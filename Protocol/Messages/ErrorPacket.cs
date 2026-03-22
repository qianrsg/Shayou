namespace Shayou.Protocol.Messages
{
    public record ErrorPacket : PacketEnvelope
    {
        public ErrorPacket()
        {
            Kind = PacketKinds.Error;
        }

        public string ErrorMessage { get; init; } = string.Empty;
    }
}
