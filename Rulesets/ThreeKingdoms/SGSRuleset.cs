using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Rulesets;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Bang.Rulesets.ThreeKingdoms
{
    public class SGSRuleset : BaseRuleset
    {
        public List<string> PlayerTurnPhase { get; private set; }
        private Dictionary<string, Action<Event, GameContext>> eventHandlers;
        private GameContext context;

        public SGSRuleset() : base("sgs")
        {
            PlayerTurnPhase = new List<string> { "Praparation", "Judging", "Draw", "Play", "Discard", "End" };
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            eventHandlers = new Dictionary<string, Action<Event, GameContext>>
            {
                { "Game_Entering", (e, ctx) => HandleGameEntering(e, ctx) },
                { "Game_Processing", (e, ctx) => HandleGameProcessing(e, ctx) },
                { "Round_Processing", (e, ctx) => HandleRoundProcessing(e, ctx) },
                { "PlayerTurn_Entering", (e, ctx) => HandlePlayerTurnEntering(e, ctx) },
                { "PlayerTurn_Processing", (e, ctx) => HandlePlayerTurnProcessing(e, ctx) },
                { "Draw_Processing", (e, ctx) => HandleDrawProcessing(e, ctx) },
                { "MoveCard_Processing", (e, ctx) => HandleMoveCardProcessing(e, ctx) }
            };
        }

        private void HandleGameEntering(Event gameEvent, GameContext context)
        {
            context.Piles.CreatePilePair("Public");
            context.Piles.LoadFromJson("Config/Piles/standard.json", "Public_Deck");
            context.Piles.Shuffle("Public_Deck");

            if (context.Players.Count == 0)
            {
                gameEvent.Stop();
                return;
            }

            foreach (var player in context.Players)
            {
                player.Draw(4);
            }
        }

        private void HandleGameProcessing(Event gameEvent, GameContext context)
        {
            Event roundEvent = new Event("Round");
            context.CreateEvent(roundEvent);
        }

        private void HandleRoundProcessing(Event gameEvent, GameContext context)
        {
            context.Round++;
            Player currentPlayer = context.Players.FirstOrDefault(p => p.Position == context.Round % context.Players.Count);
            if (currentPlayer != null)
            {
                context.CurrentPlayer = currentPlayer;
                Event playerTurnEvent = new Event("PlayerTurn");
                context.CreateEvent(playerTurnEvent);
            }
        }

        private void HandlePlayerTurnEntering(Event gameEvent, GameContext context)
        {
            if (context.CurrentPlayer != null)
            {
                context.CurrentPlayer.TurnPhase.Clear();
                foreach (var phase in PlayerTurnPhase)
                {
                    context.CurrentPlayer.TurnPhase.Add(phase);
                }
            }
        }

        private void HandlePlayerTurnProcessing(Event gameEvent, GameContext context)
        {
            if (context.CurrentPlayer != null && context.CurrentPlayer.TurnPhase.Count > 0)
            {
                while (context.CurrentPlayer.TurnPhase.Count > 0)
                {
                    string currentPhase = context.CurrentPlayer.TurnPhase[0];
                    context.CurrentPlayer.TurnPhase.RemoveAt(0);
                    Event phaseEvent = new Event(currentPhase);
                    if (currentPhase == "Draw")
                    {
                        phaseEvent.Num = 2;
                    }
                    context.CreateEvent(phaseEvent);
                }
            }
        }

        private void HandleDrawProcessing(Event gameEvent, GameContext context)
        {
            if (context.CurrentPlayer != null && gameEvent.Num > 0)
            {
                context.CurrentPlayer.Draw(gameEvent.Num);
            }
        }

        private void HandleMoveCardProcessing(Event gameEvent, GameContext context)
        {
            List<Card> sourceContainer = gameEvent.SourceContainer;
            List<Card> targetContainer = gameEvent.TargetContainer;

            if (gameEvent.Cards != null)
            {
                foreach (var card in gameEvent.Cards)
                {
                    sourceContainer.Remove(card);
                    targetContainer.Add(card);
                }
            }

            if (gameEvent.Cards != null && gameEvent.Cards.Count > 0)
            {
                string indent = new string(' ', (context.Engine.StackDepth + 1) * 2);
                
                if (sourceContainer.Count > 0 && sourceContainer[0].PileName.Contains("Deck") && targetContainer == gameEvent.SourcePlayer?.Hand)
                {
                    foreach (var card in gameEvent.Cards)
                    {
                        Console.WriteLine($"{indent}{gameEvent.SourcePlayer?.HeroName}摸到了: {card.Name} {card.Suit} {card.Rank}");
                    }
                }
                else if (sourceContainer == gameEvent.SourcePlayer?.Hand && targetContainer.Count > 0 && targetContainer[0].PileName.Contains("Discard"))
                {
                    Console.WriteLine($"{indent}{gameEvent.SourcePlayer?.HeroName}弃置了{gameEvent.Cards.Count}张牌");
                }
            }
        }

        public override void Initialize(GameContext context)
        {
            this.context = context;
            Console.WriteLine("Initializing SGS ruleset");
            Console.WriteLine("   Setting up game with " + context.Players.Count + " players");
        }

        public override void EventHandler(Event gameEvent, GameContext context)
        {
            string indent = new string(' ', context.Engine.StackDepth * 2);
            Console.WriteLine($"{indent}Event: {gameEvent.Name}, Process: {gameEvent.Process}");

            string handlerKey = $"{gameEvent.Name}_{gameEvent.Process}";
            if (eventHandlers.ContainsKey(handlerKey))
            {
                Thread.Sleep(10);
                eventHandlers[handlerKey](gameEvent, context);
            }
        }

        public override void GameStart(GameContext context)
        {
            for (int i = 0; i < 8; i++)
            {
                Player player = new Player(4, i, $"Player{i + 1}");
                player.Context = context;
                player.Pile = "Public";
                player.MaxHealth = 4;
                player.Health = 4;
                context.Players.Add(player);
            }

            Event gameEvent = new Event("Game");
            context.CreateEvent(gameEvent);
        }
    }
}
