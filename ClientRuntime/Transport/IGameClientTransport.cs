using Shayou.Protocol.Messages;

namespace Shayou.ClientRuntime.Transport
{
    public interface IGameClientTransport
    {
        void SendPacket(PacketEnvelope packet);

        PacketEnvelope WaitForPacket();
    }
}
