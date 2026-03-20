using Shayou.Core.Domain.Entities;
using Shayou.Core.StateMachine;
using Shayou.Infrastructure.Interaction;
using Shayou.Infrastructure.Interaction.Contracts;
using Shayou.Rulesets.ThreeKingdoms;
using System;
using System.Text;
using System.Threading;

namespace Shayou
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== Game Engine Test ===\n");

            SGSRuleset ruleset = new SGSRuleset();
            GameEngine engine = new GameEngine(ruleset);

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
                string input = Console.ReadLine();
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
