using Shayou.Engine.Core.Runtime;
using Shayou.Gameplay.Rulesets.ThreeKingdoms;
using Shayou.Host.Local.Transport;
using Shayou.Protocol.Messages;
using System;
using System.Text;
using System.Threading;

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
            GameEngine engine = new GameEngine(
                ruleset,
                transport.CreateServerConnection(),
                transport.CreateClientConnection());

            Thread frontendThread = new Thread(() =>
            {
                while (true)
                {
                    PacketEnvelope packet = engine.ClientConnection.WaitForPacket();

                    if (packet is InputRequestPacket inputRequestPacket)
                    {
                        ConsoleLogger.Log(
                            LogChannel.Frontend,
                            $"收到输入请求: {inputRequestPacket.RequestKey}");
                    }
                }
            });
            frontendThread.IsBackground = true;
            frontendThread.Start();

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

                ConsoleLogger.Log(LogChannel.Frontend, $"提交输入: {input}");
                engine.ClientConnection.SendInput(input);
            }

            Console.WriteLine("\n=== Test Completed ===");
        }
    }
}
