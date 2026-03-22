using Shayou.ClientRuntime.Api;
using Shayou.ClientRuntime.Messaging;
using Shayou.ClientRuntime.State;
using Shayou.ClientRuntime.Transport;
using Shayou.Protocol.Messages;

namespace Shayou.ClientRuntime.Runtime
{
    public class GameClientRuntime
    {
        private readonly IGameClientTransport transport;
        private Thread? receiveThread;

        public GameClientRuntime(IGameClientTransport transport)
        {
            this.transport = transport;
            Bus = new GameClientMessageBus();
            Api = new GameClientApi(transport);
            Session = new SessionState();
            Room = new RoomState();
            Game = new GameState();
        }

        public IGameClientMessageBus Bus { get; }

        public IGameClientApi Api { get; }

        public SessionState Session { get; }

        public RoomState Room { get; }

        public GameState Game { get; }

        public void Start()
        {
            if (receiveThread != null)
            {
                return;
            }

            receiveThread = new Thread(ReceiveLoop)
            {
                IsBackground = true
            };
            receiveThread.Start();
        }

        private void ReceiveLoop()
        {
            while (true)
            {
                PacketEnvelope packet = transport.WaitForPacket();
                Publish(packet);
            }
        }

        private void Publish(PacketEnvelope packet)
        {
            if (packet is PacketBatch batchPacket)
            {
                foreach (PacketEnvelope message in batchPacket.Messages)
                {
                    Publish(message);
                }

                return;
            }

            Session.LastKind = packet.Kind;
            Session.LastKey = packet.Key;

            if (packet.Key.StartsWith("room.", StringComparison.Ordinal))
            {
                Room.LastKey = packet.Key;
            }

            if (packet.Key.StartsWith("game.", StringComparison.Ordinal))
            {
                Game.LastKey = packet.Key;
            }

            Bus.Publish(packet);
        }
    }
}
