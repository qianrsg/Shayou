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

        public override void Initialize()
        {
            Console.WriteLine("Initializing Bang ruleset");
            Console.WriteLine("   Setting up game with " + Context.GetPlayers().Count + " players");
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
