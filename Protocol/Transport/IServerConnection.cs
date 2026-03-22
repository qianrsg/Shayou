using Shayou.Protocol.Messages;

namespace Shayou.Protocol.Transport
{
    public interface IServerConnection
    {
        void SendPacket(PacketEnvelope packet);

        PacketEnvelope WaitForPacket();
    }
}
