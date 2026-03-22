using Shayou.Protocol.Messages;
using Shayou.Protocol.Serialization;
using Shayou.Protocol.Transport;
using System.Collections.Generic;
using System.Threading;

namespace Shayou.Host.Local.Transport
{
    public class LocalLoopbackTransport
    {
        private readonly LocalMessageQueue<string> _serverToClientQueue;
        private readonly LocalMessageQueue<string> _clientToServerQueue;

        public LocalLoopbackTransport()
        {
            _serverToClientQueue = new LocalMessageQueue<string>();
            _clientToServerQueue = new LocalMessageQueue<string>();
        }

        public IServerConnection CreateServerConnection()
        {
            return new LocalServerConnection(_serverToClientQueue, _clientToServerQueue);
        }

        public IClientConnection CreateClientConnection()
        {
            return new LocalClientConnection(_serverToClientQueue, _clientToServerQueue);
        }
    }

    internal class LocalServerConnection : IServerConnection
    {
        private readonly LocalMessageQueue<string> _serverToClientQueue;
        private readonly LocalMessageQueue<string> _clientToServerQueue;

        public LocalServerConnection(
            LocalMessageQueue<string> serverToClientQueue,
            LocalMessageQueue<string> clientToServerQueue)
        {
            _serverToClientQueue = serverToClientQueue;
            _clientToServerQueue = clientToServerQueue;
        }

        public void SendPacket(PacketEnvelope packet)
        {
            string packetJson = PacketJsonSerializer.Serialize(packet);
            _serverToClientQueue.Send(packetJson);
        }

        public PacketEnvelope WaitForPacket()
        {
            string packetJson = _clientToServerQueue.Receive();
            return PacketJsonSerializer.Deserialize(packetJson);
        }
    }

    internal class LocalClientConnection : IClientConnection
    {
        private readonly LocalMessageQueue<string> _serverToClientQueue;
        private readonly LocalMessageQueue<string> _clientToServerQueue;

        public LocalClientConnection(
            LocalMessageQueue<string> serverToClientQueue,
            LocalMessageQueue<string> clientToServerQueue)
        {
            _serverToClientQueue = serverToClientQueue;
            _clientToServerQueue = clientToServerQueue;
        }

        public PacketEnvelope WaitForPacket()
        {
            string packetJson = _serverToClientQueue.Receive();
            return PacketJsonSerializer.Deserialize(packetJson);
        }

        public void SendPacket(PacketEnvelope packet)
        {
            string packetJson = PacketJsonSerializer.Serialize(packet);
            _clientToServerQueue.Send(packetJson);
        }
    }

    internal class LocalMessageQueue<T>
    {
        private readonly Queue<T> _messages;
        private readonly object _syncRoot;
        private readonly SemaphoreSlim _messageCount;

        public LocalMessageQueue()
        {
            _messages = new Queue<T>();
            _syncRoot = new object();
            _messageCount = new SemaphoreSlim(0);
        }

        public void Send(T message)
        {
            lock (_syncRoot)
            {
                _messages.Enqueue(message);
            }

            _messageCount.Release();
        }

        public T Receive()
        {
            _messageCount.Wait();

            lock (_syncRoot)
            {
                return _messages.Dequeue();
            }
        }
    }
}
