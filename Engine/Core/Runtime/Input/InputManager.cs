using Shayou.Engine.Foundations.Input;
using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;

namespace Shayou.Engine.Core.Runtime.Input
{
    public class InputManager : IInputService
    {
        private readonly IServerConnection serverConnection;
        public string? CurrentContextKey { get; private set; }

        public InputManager(IServerConnection serverConnection)
        {
            this.serverConnection = serverConnection;
        }

        public InputSubmission WaitForInput(InputRequest request)
        {
            CurrentContextKey = request.Key;

            try
            {
                while (true)
                {
                    PacketEnvelope packet = serverConnection.WaitForPacket();

                    if (packet is not CommandPacket commandPacket)
                    {
                        throw new InvalidOperationException(
                            $"Expected command packet from client, but received '{packet.Kind}:{packet.Key}'.");
                    }

                    InputSubmission submission = new()
                    {
                        ActionKey = commandPacket.Key
                    };

                    InputCheckResult checkResult = request.CheckInput(submission);
                    if (checkResult.IsValid)
                    {
                        return submission;
                    }
                }
            }
            finally
            {
                CurrentContextKey = null;
            }
        }
    }
}
