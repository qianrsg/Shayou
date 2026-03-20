using Shayou.Engine.Core.Domain.Entities;
using Shayou.Engine.Rulesets;

namespace Shayou.Engine.Rulesets.Bang
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

        public override RulesetRegistrations GetRegistrations()
        {
            return new RulesetRegistrations();
        }

        public override void SetupGame()
        {
        }
    }
}
