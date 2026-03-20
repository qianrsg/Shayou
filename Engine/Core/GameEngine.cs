using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Core.Domain.Models;
using Shayou.Engine.Interaction;
using Shayou.Engine.Rulesets;
using Shayou.Host.Local.Transport;
using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;

namespace Shayou.Engine.Core.StateMachine
{
    public class GameEngine
    {
        public GameContext Context { get; private set; }
        public BaseRuleset Ruleset { get; private set; }
        public InputManager InputManager { get; private set; }
        public IClientConnection ClientConnection { get; private set; }
        private IServerConnection ServerConnection { get; set; }
        private Stack<BaseEvent> eventStack;
        public int StackDepth { get; private set; }

        public GameEngine(BaseRuleset ruleset)
        {
            Context = new GameContext();
            Context.Engine = this;
            Ruleset = ruleset;
            Ruleset.Engine = this;
            RegisterRulesetRegistrations();
            Ruleset.Initialize();
            eventStack = new Stack<BaseEvent>();
            StackDepth = 0;
            LocalLoopbackTransport transport = new LocalLoopbackTransport();
            ServerConnection = transport.CreateServerConnection();
            ClientConnection = transport.CreateClientConnection();
            InputManager = new InputManager(ServerConnection);
        }

        private void RegisterRulesetRegistrations()
        {
            RulesetRegistrations registrations = Ruleset.GetRegistrations();

            foreach (var validatorPair in registrations.UiChoiceValidators)
            {
                Context.Registry.UiChoiceValidators.Register(
                    validatorPair.Key,
                    validatorPair.Value);
            }

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
                Thread.Sleep(10);
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

        public void CreateEvent(BaseEvent gameEvent)
        {
            AttachEventCallback(gameEvent);
            eventStack.Push(gameEvent);
            StackDepth++;

            while (!gameEvent.IsEnd())
            {
                ExecuteTimingRuleAction(gameEvent);
                gameEvent.AdvanceProcess();
            }

            eventStack.Pop();
            StackDepth--;
        }

        public void GameStart()
        {
            Thread gameThread = new Thread(() =>
            {
                Ruleset.SetupGame();
                CreateEvent(new Event("Game"));
            });
            gameThread.Start();
        }

        public string WaitForInput(InputRequestPacket requestPacket)
        {
            return InputManager.WaitForInput(requestPacket);
        }
    }
}
