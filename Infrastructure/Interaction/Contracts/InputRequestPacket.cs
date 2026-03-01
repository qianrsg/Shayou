namespace Shayou.Infrastructure.Interaction.Contracts
{
    public record InputRequestPacket : PacketEnvelope
    {
        public required string WindowId { get; init; }
        public required string PlayerId { get; init; }
        public bool CanBeCancelled { get; init; } = true;
    }
}
