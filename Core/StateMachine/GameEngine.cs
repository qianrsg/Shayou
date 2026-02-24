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
        private Stack<Event> eventStack;
        public int StackDepth { get; private set; }

        public GameEngine(BaseRuleset ruleset)
        {
            Context = new GameContext();
            Context.Engine = this;
            Ruleset = ruleset;
            eventStack = new Stack<Event>();
            StackDepth = 0;
        }

        public void CreateEvent(Event gameEvent)
        {
            eventStack.Push(gameEvent);
            StackDepth++;
            
            do
            {
                Ruleset.EventHandler(gameEvent, Context);
                
                if (!gameEvent.IsFinished())
                {
                    gameEvent.AdvanceProcess();
                }
            } while (!gameEvent.IsFinished());
            
            eventStack.Pop();
            StackDepth--;
        }

        public void GameStart()
        {
            Thread gameThread = new Thread(() => Ruleset.GameStart(Context));
            gameThread.Start();
        }
    }
}