using Shayou.Engine.Core.Runtime.Context;
using Shayou.Engine.Foundations.Events;
using Shayou.Engine.Foundations.Input;

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

        protected InputSubmission RequestInput(InputRequest request)
        {
            return Context.Services.InputService.WaitForInput(request);
        }

        protected InputSubmission RequestInput(string key)
        {
            InputRequest request = Context.Registry.InputRequests.Create(key);
            return RequestInput(request);
        }

        public abstract void Initialize();

        public abstract RulesetRegistrations GetRegistrations();

        public abstract void SetupGame();
    }
}
