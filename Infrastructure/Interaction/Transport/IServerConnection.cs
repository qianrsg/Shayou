using Shayou.Infrastructure.Interaction.Contracts;

namespace Shayou.Infrastructure.Interaction.Transport
{
    public interface IServerConnection
    {
        void SendPacket(PacketEnvelope packet);

        string WaitForInput();
    }
}
