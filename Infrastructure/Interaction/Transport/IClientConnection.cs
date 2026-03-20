using Shayou.Infrastructure.Interaction.Contracts;

namespace Shayou.Infrastructure.Interaction.Transport
{
    public interface IClientConnection
    {
        PacketEnvelope WaitForPacket();

        void SendInput(string input);
    }
}
