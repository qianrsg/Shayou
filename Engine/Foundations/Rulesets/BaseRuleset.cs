using Shayou.Engine.Core.Runtime.Context;
using Shayou.Engine.Foundations.Events;
using Shayou.Protocol.Messages;

namespace Shayou.Engine.Foundations.Rulesets
{
    public abstract class BaseRuleset
    {
        public string Id { get; protected set; }
        public GameContext Context { get; set; } = null!;

        public BaseRuleset(string id)
        {
            Id = id;
        }

        protected void EmitEvent(BaseEvent gameEvent)
        {
            Context.Services.EventDispatcher.DispatchEvent(gameEvent);
        }

        protected string RequestInput(InputRequestPacket requestPacket)
        {
            return Context.Services.InputService.RequestInput(requestPacket);
        }

        public abstract void Initialize();

        public abstract RulesetRegistrations GetRegistrations();

        public abstract void SetupGame();
    }
}
