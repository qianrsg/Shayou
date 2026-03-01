using Shayou.Core.Domain.Entities;
using Shayou.Core.StateMachine;
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

            Console.WriteLine("Starting game...");
            engine.GameStart();

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                {
                    break;
                }
                engine.PostInput(input);
            }

            Console.WriteLine("\n=== Test Completed ===");
        }
    }
}
