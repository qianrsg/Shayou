using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;

namespace Bang.Rulesets
{
    public abstract class BaseRuleset
    {
        public string Id { get; protected set; }

        public BaseRuleset(string id)
        {
            Id = id;
        }

        public abstract void Initialize(GameContext context);

        public abstract void EventHandler(Event gameEvent, GameContext context);

        public abstract void GameStart(GameContext context);
    }
}