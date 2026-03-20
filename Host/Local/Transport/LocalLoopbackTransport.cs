using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;
using System.Collections.Generic;
using System.Threading;

namespace Shayou.Host.Local.Transport
{
    public class LocalLoopbackTransport
    {
        private readonly LocalMessageQueue<PacketEnvelope> _serverToClientQueue;
        private readonly LocalMessageQueue<string> _clientToServerQueue;

        public LocalLoopbackTransport()
        {
            _serverToClientQueue = new LocalMessageQueue<PacketEnvelope>();
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
        private readonly LocalMessageQueue<PacketEnvelope> _serverToClientQueue;
        private readonly LocalMessageQueue<string> _clientToServerQueue;

        public LocalServerConnection(
            LocalMessageQueue<PacketEnvelope> serverToClientQueue,
            LocalMessageQueue<string> clientToServerQueue)
        {
            _serverToClientQueue = serverToClientQueue;
            _clientToServerQueue = clientToServerQueue;
        }

        public void SendPacket(PacketEnvelope packet)
        {
            _serverToClientQueue.Send(packet);
        }

        public string WaitForInput()
        {
            return _clientToServerQueue.Receive();
        }
    }

    internal class LocalClientConnection : IClientConnection
    {
        private readonly LocalMessageQueue<PacketEnvelope> _serverToClientQueue;
        private readonly LocalMessageQueue<string> _clientToServerQueue;

        public LocalClientConnection(
            LocalMessageQueue<PacketEnvelope> serverToClientQueue,
            LocalMessageQueue<string> clientToServerQueue)
        {
            _serverToClientQueue = serverToClientQueue;
            _clientToServerQueue = clientToServerQueue;
        }

        public PacketEnvelope WaitForPacket()
        {
            return _serverToClientQueue.Receive();
        }

        public void SendInput(string input)
        {
            _clientToServerQueue.Send(input);
        }
    }

    internal class LocalMessageQueue<T>
    {
        private readonly Queue<T> _messages;
        private readonly Mutex _mutex;
        private readonly AutoResetEvent _messageReadyEvent;

        public LocalMessageQueue()
        {
            _messages = new Queue<T>();
            _mutex = new Mutex();
            _messageReadyEvent = new AutoResetEvent(false);
        }

        public void Send(T message)
        {
            _mutex.WaitOne();
            try
            {
                _messages.Enqueue(message);
                _messageReadyEvent.Set();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        public T Receive()
        {
            _mutex.WaitOne();
            try
            {
                if (_messages.Count > 0)
                {
                    return _messages.Dequeue();
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }

            _messageReadyEvent.WaitOne();

            _mutex.WaitOne();
            try
            {
                return _messages.Dequeue();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }
    }
}
