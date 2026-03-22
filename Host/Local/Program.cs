using Shayou.ClientRuntime.Api;
using Shayou.ClientRuntime.Runtime;
using Shayou.ClientRuntime.Transport;
using Shayou.Engine.Core.Runtime;
using Shayou.Gameplay.Rulesets.ThreeKingdoms;
using Shayou.Host.Local.Transport;
using Shayou.Protocol.Messages;
using System;
using System.Text;

namespace Shayou.Host.Local
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Game Engine Test ===\n");

            SGSRuleset ruleset = new SGSRuleset();
            LocalLoopbackTransport transport = new LocalLoopbackTransport();
            var clientConnection = transport.CreateClientConnection();
            GameEngine engine = new GameEngine(
                ruleset,
                transport.CreateServerConnection(),
                clientConnection);

            GameClientRuntime clientRuntime = new GameClientRuntime(
                new ClientConnectionTransport(clientConnection));
            clientRuntime.Bus.Subscribe(PacketKinds.Event, null, packet => HandlePacket(packet, clientRuntime));
            clientRuntime.Bus.Subscribe(PacketKinds.Response, null, packet => HandlePacket(packet, clientRuntime));
            clientRuntime.Bus.Subscribe(PacketKinds.Snapshot, null, packet => HandlePacket(packet, clientRuntime));
            clientRuntime.Bus.Subscribe(PacketKinds.Error, null, packet => HandlePacket(packet, clientRuntime));
            clientRuntime.Start();

            Console.WriteLine("Starting game...");
            engine.GameStart();

            while (true)
            {
                string? input = Console.ReadLine();
                if (input == null)
                {
                    break;
                }

                if (input == "exit")
                {
                    break;
                }

                SubmitConsoleCommand(input, clientRuntime.Api);
            }

            Console.WriteLine("\n=== Test Completed ===");
        }

        private static void SubmitConsoleCommand(string input, IGameClientApi api)
        {
            string normalized = input.Trim();

            switch (normalized)
            {
                case "pass":
                    ConsoleLogger.Log(LogChannel.Frontend, "submit command: game.Pass");
                    api.Pass();
                    return;
                case "confirm":
                    ConsoleLogger.Log(LogChannel.Frontend, "submit command: game.Confirm");
                    api.Confirm();
                    return;
                case "cancel":
                    ConsoleLogger.Log(LogChannel.Frontend, "submit command: game.Cancel");
                    api.Cancel();
                    return;
                default:
                    ConsoleLogger.Log(LogChannel.Frontend, $"submit command: {normalized}");
                    api.SendCommand(normalized);
                    return;
            }
        }

        private static void HandlePacket(PacketEnvelope packet, GameClientRuntime clientRuntime)
        {
            string keyDescription = DescribeKey(packet.Key);
            string stateDescription =
                $"session={clientRuntime.Session.LastKind}/{clientRuntime.Session.LastKey}, " +
                $"room={FormatStateValue(clientRuntime.Room.LastKey)}, " +
                $"game={FormatStateValue(clientRuntime.Game.LastKey)}";

            if (packet is EventPacket eventPacket)
            {
                ConsoleLogger.Log(
                    LogChannel.Frontend,
                    $"event: type={eventPacket.GetType().Name}, kind={eventPacket.Kind}, key={eventPacket.Key} ({keyDescription}), sourcePlayer={FormatValue(eventPacket.SourcePlayer)}, targetPlayer={FormatValue(eventPacket.TargetPlayer)}, sourceCard={FormatValue(eventPacket.SourceCard)}, cards={FormatList(eventPacket.Cards)}, num={FormatValue(eventPacket.Num)}, players={FormatList(eventPacket.Players)}, nums={FormatList(eventPacket.Nums)}, state=[{stateDescription}]");

                return;
            }

            if (packet is ResponsePacket responsePacket)
            {
                ConsoleLogger.Log(
                    LogChannel.Frontend,
                    $"response: type={packet.GetType().Name}, kind={responsePacket.Kind}, key={responsePacket.Key} ({keyDescription}), success={responsePacket.Success}, state=[{stateDescription}]");

                return;
            }

            if (packet is ErrorPacket errorPacket)
            {
                ConsoleLogger.Log(
                    LogChannel.Frontend,
                    $"error: type={packet.GetType().Name}, kind={errorPacket.Kind}, key={errorPacket.Key} ({keyDescription}), message={errorPacket.ErrorMessage}, state=[{stateDescription}]");

                return;
            }

            ConsoleLogger.Log(
                LogChannel.Frontend,
                $"{packet.Kind}: type={packet.GetType().Name}, key={packet.Key} ({keyDescription}), state=[{stateDescription}]");
        }

        private static string DescribeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "scope=-, action=-";
            }

            string[] parts = key.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return $"scope={parts[0]}, action=-";
            }

            string scope = parts[0];
            string action = string.Join('.', parts, 1, parts.Length - 1);
            return $"scope={scope}, action={action}";
        }

        private static string FormatStateValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static string FormatValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static string FormatList<T>(IReadOnlyCollection<T> values)
        {
            return values.Count == 0 ? "-" : string.Join(",", values);
        }
    }
}
