using Shayou.Core.Domain.Entities;
using Shayou.Core.Domain.Models;
using Shayou.Infrastructure.Interaction;
using Shayou.Rulesets;
using System.Collections.Generic;

namespace Shayou.Core.StateMachine
{
    public class GameEngine
    {
        public GameContext Context { get; private set; }
        public BaseRuleset Ruleset { get; private set; }
        public InputManager InputManager { get; private set; }
        private Stack<BaseEvent> eventStack;
        public int StackDepth { get; private set; }

        public GameEngine(BaseRuleset ruleset)
        {
            Context = new GameContext();
            Context.Engine = this;
            Ruleset = ruleset;
            Ruleset.Engine = this;
            Ruleset.Initialize();
            eventStack = new Stack<BaseEvent>();
            StackDepth = 0;
            InputManager = new InputManager();
        }

        public void CreateEvent(BaseEvent gameEvent)
        {
            Ruleset.PrepareEvent(gameEvent);
            eventStack.Push(gameEvent);
            StackDepth++;

            while (!gameEvent.IsEnd())
            {
                Ruleset.EventHandler(gameEvent);
                gameEvent.AdvanceProcess();
            }

            eventStack.Pop();
            StackDepth--;
        }

        public void GameStart()
        {
            Thread gameThread = new Thread(() => Ruleset.GameStart());
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

        public void MoveCard(List<Card> cards, List<Card> source, List<Card> target)
        {
            AtomicEvent atomicEvent = new AtomicEvent("MoveCard");
            atomicEvent.Cards = cards;
            atomicEvent.SourceContainer = source;
            atomicEvent.TargetContainer = target;
            atomicEvent.Callback = (e) =>
            {
                foreach (var card in cards)
                {
                    source.Remove(card);
                    target.Add(card);
                }
            };
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

        public void PostInput(string input)
        {
            InputManager.PostInput(input);
        }

        public string WaitForInput()
        {
            return InputManager.WaitForInput();
        }
    }
}
