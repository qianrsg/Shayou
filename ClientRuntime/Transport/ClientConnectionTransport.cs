using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;

namespace Shayou.ClientRuntime.Transport
{
    public class ClientConnectionTransport : IGameClientTransport
    {
        private readonly IClientConnection clientConnection;

        public ClientConnectionTransport(IClientConnection clientConnection)
        {
            this.clientConnection = clientConnection;
        }

        public void SendPacket(PacketEnvelope packet)
        {
            clientConnection.SendPacket(packet);
        }

        public PacketEnvelope WaitForPacket()
        {
            return clientConnection.WaitForPacket();
        }
    }
}
