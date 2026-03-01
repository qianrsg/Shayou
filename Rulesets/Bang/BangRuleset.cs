using Shayou.Core.Domain.Entities;
using Shayou.Rulesets;

namespace Shayou.Rulesets.Bang
{
    public class BangRuleset : BaseRuleset
    {
        public BangRuleset() : base("bang")
        {
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing Bang ruleset");
            Console.WriteLine("   Setting up game with " + Engine.Context.Players.Count + " players");
        }

        public override void EventHandler(BaseEvent gameEvent)
        {
            Console.WriteLine("Handling events for Bang ruleset");
            Console.WriteLine("   Processing card effects and abilities");
        }

        public override void GameStart()
        {
            Initialize();
        }
    }
}
