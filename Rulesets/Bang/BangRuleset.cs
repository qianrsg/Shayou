using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Rulesets;

namespace Bang.Rulesets.Bang
{
    public class BangRuleset : BaseRuleset
    {
        public BangRuleset() : base("bang")
        {
        }

        public override void Initialize(GameContext context)
        {
            Console.WriteLine("Initializing Bang ruleset");
            Console.WriteLine("   Setting up game with " + context.Players.Count + " players");
        }

        public override void EventHandler(Event gameEvent, GameContext context)
        {
            Console.WriteLine("Handling events for Bang ruleset");
            Console.WriteLine("   Processing card effects and abilities");
        }

        public override void GameStart(GameContext context)
        {
            Initialize(context);
        }
    }
}