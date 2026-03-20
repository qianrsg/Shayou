using Shayou.Protocol.Messages;

namespace Shayou.Protocol.Transport
{
    public interface IClientConnection
    {
        PacketEnvelope WaitForPacket();

        void SendInput(string input);
    }
}
