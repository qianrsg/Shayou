using Shayou.Protocol.Messages;

namespace Shayou.ClientRuntime.Messaging
{
    public interface IGameClientMessageBus
    {
        void Publish(PacketEnvelope packet);

        IDisposable Subscribe(string kind, string? key, Action<PacketEnvelope> handler);
    }
}
