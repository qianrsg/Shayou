namespace Shayou.Protocol.Messages
{
    public record SnapshotPacket : PacketEnvelope
    {
        public SnapshotPacket()
        {
            Kind = PacketKinds.Snapshot;
        }
    }
}
