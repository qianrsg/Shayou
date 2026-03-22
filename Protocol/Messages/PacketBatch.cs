using System.Collections.Generic;

namespace Shayou.Protocol.Messages
{
    public record PacketBatch : PacketEnvelope
    {
        public PacketBatch()
        {
            Kind = PacketKinds.Batch;
            Key = PacketKinds.Batch;
        }

        public List<PacketEnvelope> Messages { get; init; } = new();
    }
}
