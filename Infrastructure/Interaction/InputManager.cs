using Shayou.Infrastructure.Interaction.Contracts;
using Shayou.Infrastructure.Interaction.Transport;

namespace Shayou.Infrastructure.Interaction
{
    public class InputManager
    {
        private readonly IServerConnection serverConnection;
        public InputRequestPacket? CurrentRequest { get; private set; }

        public InputManager(IServerConnection serverConnection)
        {
            this.serverConnection = serverConnection;
        }

        public string WaitForInput(InputRequestPacket requestPacket)
        {
            CurrentRequest = requestPacket;
            serverConnection.SendPacket(requestPacket);

            string input = serverConnection.WaitForInput();
            CurrentRequest = null;
            return input;
        }
    }
}
