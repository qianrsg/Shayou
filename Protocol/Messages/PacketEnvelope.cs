namespace Shayou.Protocol.Messages
{
    public abstract record PacketEnvelope
    {
        public string Kind { get; init; } = string.Empty;

        public string Key { get; init; } = string.Empty;
    }

    public static class PacketKinds
    {
        public const string Batch = "batch";
        public const string Command = "command";
        public const string Response = "response";
        public const string Event = "event";
        public const string Snapshot = "snapshot";
        public const string Error = "error";
    }
}
