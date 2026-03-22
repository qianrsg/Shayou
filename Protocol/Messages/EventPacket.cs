using System.Collections.Generic;

namespace Shayou.Protocol.Messages
{
    public record EventPacket : PacketEnvelope
    {
        public EventPacket()
        {
            Kind = PacketKinds.Event;
        }

        public string? SourceCard { get; init; }

        public List<string> Cards { get; init; } = new();

        public string? SourcePlayer { get; init; }

        public string? TargetPlayer { get; init; }

        public string? Num { get; init; }

        public List<string> Players { get; init; } = new();

        public List<int> Nums { get; init; } = new();
    }
}
