using Shayou.ClientRuntime.Transport;
using Shayou.Protocol.Messages;

namespace Shayou.ClientRuntime.Api
{
    public class GameClientApi : IGameClientApi
    {
        private readonly IGameClientTransport transport;

        public GameClientApi(IGameClientTransport transport)
        {
            this.transport = transport;
        }

        public void SendCommand(string key)
        {
            transport.SendPacket(new CommandPacket
            {
                Key = key
            });
        }

        public void CreateRoom()
        {
            SendCommand("room.Create");
        }

        public void JoinRoom()
        {
            SendCommand("room.Join");
        }

        public void Ready()
        {
            SendCommand("room.Ready");
        }

        public void CancelReady()
        {
            SendCommand("room.CancelReady");
        }

        public void StartGame()
        {
            SendCommand("room.StartGame");
        }

        public void Pass()
        {
            SendCommand("game.Pass");
        }

        public void Confirm()
        {
            SendCommand("game.Confirm");
        }

        public void Cancel()
        {
            SendCommand("game.Cancel");
        }
    }
}
