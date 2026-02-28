using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Rulesets;
using System.Collections.Generic;
using System.Threading;

namespace Bang.Core.StateMachine
{
    public class GameEngine
    {
        public GameContext Context { get; private set; }
        public BaseRuleset Ruleset { get; private set; }
        private Stack<BaseEvent> eventStack;
        public int StackDepth { get; private set; }

        public GameEngine(BaseRuleset ruleset)
        {
            Context = new GameContext();
            Context.Engine = this;
            Ruleset = ruleset;
            Ruleset.Initialize(Context);
            eventStack = new Stack<BaseEvent>();
            StackDepth = 0;
        }

        public void CreateEvent(BaseEvent gameEvent)
        {
            eventStack.Push(gameEvent);
            StackDepth++;
            
            while (!gameEvent.IsEnd())
            {
                Ruleset.EventHandler(gameEvent, Context);
                gameEvent.AdvanceProcess();
            }
            
            eventStack.Pop();
            StackDepth--;
        }

        public void GameStart()
        {
            Thread gameThread = new Thread(() => Ruleset.GameStart(Context));
            gameThread.Start();
        }

        public void ChangeHealth(Player player, int num)
        {
            AtomicEvent atomicEvent = new AtomicEvent("ChangeHealth");
            atomicEvent.SourcePlayer = player;
            atomicEvent.Num = num;
            atomicEvent.Callback = (e) =>
            {
                player.Health += num;
            };
            CreateEvent(atomicEvent);
        }

        public void MoveCard(string source, string target)
        {
            AtomicEvent atomicEvent = new AtomicEvent("MoveCard");
            atomicEvent.Data["Source"] = source;
            atomicEvent.Data["Target"] = target;
            CreateEvent(atomicEvent);
        }

        public void ChangeMaxHealth(Player player, int num)
        {
            AtomicEvent atomicEvent = new AtomicEvent("ChangeMaxHealth");
            atomicEvent.SourcePlayer = player;
            atomicEvent.Num = num;
            atomicEvent.Callback = (e) =>
            {
                player.MaxHealth += num;
            };
            CreateEvent(atomicEvent);
        }

        public void ChangeArmor(Player player, int num)
        {
            AtomicEvent atomicEvent = new AtomicEvent("ChangeArmor");
            atomicEvent.SourcePlayer = player;
            atomicEvent.Num = num;
            atomicEvent.Callback = (e) =>
            {
                player.Armor += num;
            };
            CreateEvent(atomicEvent);
        }
    }
}