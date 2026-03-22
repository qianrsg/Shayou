namespace Shayou.Protocol.Messages
{
    public record CommandPacket : PacketEnvelope
    {
        public CommandPacket()
        {
            Kind = PacketKinds.Command;
        }
    }
}
