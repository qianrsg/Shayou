using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Core.StateMachine;

namespace Bang.Rulesets
{
    public abstract class BaseRuleset
    {
        public string Id { get; protected set; }
        public GameEngine Engine { get; set; }
        public IContext Context { get; set; }

        public BaseRuleset(string id)
        {
            Id = id;
        }

        public abstract void Initialize();

        public virtual void PrepareEvent(BaseEvent gameEvent) { }

        public abstract void EventHandler(BaseEvent gameEvent);

        public abstract void GameStart();
    }
}
