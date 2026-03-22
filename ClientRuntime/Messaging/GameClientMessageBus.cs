using Shayou.Protocol.Messages;

namespace Shayou.ClientRuntime.Messaging
{
    public class GameClientMessageBus : IGameClientMessageBus
    {
        private readonly List<Subscription> subscriptions = new();
        private readonly object syncRoot = new();

        public void Publish(PacketEnvelope packet)
        {
            List<Action<PacketEnvelope>> handlers = new();

            lock (syncRoot)
            {
                foreach (Subscription subscription in subscriptions)
                {
                    if (!string.Equals(subscription.Kind, packet.Kind, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (subscription.Key != null
                        && !string.Equals(subscription.Key, packet.Key, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    handlers.Add(subscription.Handler);
                }
            }

            foreach (Action<PacketEnvelope> handler in handlers)
            {
                handler(packet);
            }
        }

        public IDisposable Subscribe(string kind, string? key, Action<PacketEnvelope> handler)
        {
            Subscription subscription = new(kind, key, handler);

            lock (syncRoot)
            {
                subscriptions.Add(subscription);
            }

            return new SubscriptionHandle(() =>
            {
                lock (syncRoot)
                {
                    subscriptions.Remove(subscription);
                }
            });
        }

        private sealed record Subscription(string Kind, string? Key, Action<PacketEnvelope> Handler);

        private sealed class SubscriptionHandle : IDisposable
        {
            private readonly Action disposeAction;
            private bool disposed;

            public SubscriptionHandle(Action disposeAction)
            {
                this.disposeAction = disposeAction;
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                disposeAction();
            }
        }
    }
}
