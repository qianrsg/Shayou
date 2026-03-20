using Shayou.Core.Domain.Entities;
using Shayou.Core.StateMachine;

namespace Shayou.Rulesets
{
    public abstract class BaseRuleset
    {
        public string Id { get; protected set; }
        public GameEngine Engine { get; set; }

        public BaseRuleset(string id)
        {
            Id = id;
        }

        public abstract void Initialize();

        public abstract RulesetRegistrations GetRegistrations();

        public abstract void SetupGame();
    }
}
