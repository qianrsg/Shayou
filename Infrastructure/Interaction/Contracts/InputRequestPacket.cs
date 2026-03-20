namespace Shayou.Infrastructure.Interaction.Contracts
{
    public record InputRequestPacket : PacketEnvelope
    {
        public required string WindowId { get; init; }
        public required string PlayerId { get; init; }
        public required string RequestKey { get; init; }
        public required string PromptKey { get; init; }
        public string? ValidatorKey { get; init; }
        public int TimeoutMs { get; init; } = 10000;
        public bool CanBeCancelled { get; init; } = true;
    }
}
