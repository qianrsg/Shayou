using Shayou.Engine.Foundations.Events;
using Shayou.Engine.Foundations.Rulesets;
using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Core.Runtime.Context;
using Shayou.Engine.Core.Runtime.Input;
using Shayou.Engine.Core.Runtime.Services;
using Shayou.Protocol.Transport;

namespace Shayou.Engine.Core.Runtime
{
    public class GameEngine
    {
        private GameContext Context { get; set; }
        private BaseRuleset Ruleset { get; set; } = null!;
        private InputManager InputManager { get; set; } = null!;
        public IClientConnection ClientConnection { get; private set; } = null!;
        private IServerConnection ServerConnection { get; set; } = null!;

        public GameEngine(
            BaseRuleset ruleset,
            IServerConnection serverConnection,
            IClientConnection clientConnection)
        {
            InitializeRuntime(serverConnection, clientConnection);
            Context = CreateContext();
            AttachRuleset(ruleset);
        }

        private GameContext CreateContext()
        {
            return new GameContext(new GameServiceRegistry(this, InputManager));
        }

        private void InitializeRuntime(
            IServerConnection serverConnection,
            IClientConnection clientConnection)
        {
            ServerConnection = serverConnection;
            ClientConnection = clientConnection;
            InputManager = new InputManager(ServerConnection);
        }

        private void AttachRuleset(BaseRuleset ruleset)
        {
            Ruleset = ruleset;
            Ruleset.Context = Context;
            RegisterRulesetRegistrations();
            Ruleset.Initialize();
        }

        private void RegisterRulesetRegistrations()
        {
            RulesetRegistrations registrations = Ruleset.GetRegistrations();

            foreach (var ruleActionPair in registrations.TimingRuleActions)
            {
                Context.Registry.TimingRuleActions.Register(
                    ruleActionPair.Key,
                    ruleActionPair.Value);
            }

            foreach (var callbackPair in registrations.EventCallbacks)
            {
                Context.Registry.EventCallbacks.Register(
                    callbackPair.Key,
                    callbackPair.Value);
            }
        }

        private void ExecuteTimingRuleAction(BaseEvent gameEvent)
        {
            string ruleActionKey = $"{gameEvent.Name}_{gameEvent.Timing}";
            IReadOnlyList<Action<BaseEvent>> ruleActions = Context.Registry.TimingRuleActions.Get(ruleActionKey);

            if (ruleActions.Count == 0)
                return;

            foreach (var ruleAction in ruleActions)
            {
                ruleAction(gameEvent);
            }
        }

        private void AttachEventCallback(BaseEvent gameEvent)
        {
            Action<BaseEvent>? callback = Context.Registry.EventCallbacks.Get(gameEvent.Name);

            if (callback != null)
            {
                gameEvent.Callback = callback;
            }
        }

        public void DispatchEvent(BaseEvent gameEvent)
        {
            AttachEventCallback(gameEvent);

            while (!gameEvent.IsEnd())
            {
                ExecuteTimingRuleAction(gameEvent);
                gameEvent.AdvanceProcess();
            }
        }

        public void GameStart()
        {
            Thread gameThread = new Thread(() =>
            {
                Ruleset.SetupGame();
                DispatchEvent(new Event("Game"));
            });
            gameThread.Start();
        }
    }
}
