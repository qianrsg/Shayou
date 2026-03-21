using Shayou.Engine.Core.Domain.Entities;
using Shayou.Engine.Foundations.Rulesets;

namespace Shayou.Gameplay.Rulesets.Bang
{
    public class BangRuleset : BaseRuleset
    {
        public BangRuleset() : base("bang")
        {
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing Bang ruleset");
            Console.WriteLine("   Setting up game with " + Context.Players.Count + " players");
        }

        public override RulesetRegistrations GetRegistrations()
        {
            return new RulesetRegistrations();
        }

        public override void SetupGame()
        {
        }
    }
}
