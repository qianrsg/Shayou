using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;

namespace Shayou.Engine.Core.Runtime.Input
{
    public class InputManager
    {
        private readonly IServerConnection serverConnection;
        public InputRequestPacket? CurrentRequest { get; private set; }

        public InputManager(IServerConnection serverConnection)
        {
            this.serverConnection = serverConnection;
        }

        public string RequestInput(InputRequestPacket requestPacket)
        {
            CurrentRequest = requestPacket;
            serverConnection.SendPacket(requestPacket);

            string input = serverConnection.WaitForInput();
            CurrentRequest = null;
            return input;
        }
    }
}
