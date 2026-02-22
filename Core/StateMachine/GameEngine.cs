using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Rulesets;

namespace Bang.Core.StateMachine
{
    public class GameEngine
    {
        public GameContext Context { get; private set; }
        public BaseRuleset Ruleset { get; private set; }

        public GameEngine(BaseRuleset ruleset)
        {
            Context = new GameContext();
            Ruleset = ruleset;
        }

        public void CreateEvent(Event gameEvent)
        {
            do
            {
                Ruleset.EventHandler(gameEvent);
                
                if (!gameEvent.IsFinished())
                {
                    gameEvent.AdvanceProcess();
                }
            } while (!gameEvent.IsFinished());
        }

        public void GameStart()
        {
            Ruleset.Initialize(Context);
            Ruleset.Pipeline(Context);
        }
    }
}