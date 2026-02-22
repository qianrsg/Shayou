using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Rulesets;
using System;
using System.Collections.Generic;

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
                { "Game_Processed", (e, ctx) => HandleGameProcessed(e, ctx) },
                { "Game_Exiting", (e, ctx) => HandleGameExiting(e, ctx) },
                { "Game_Finished", (e, ctx) => HandleGameFinished(e, ctx) },
                { "Round_Entering", (e, ctx) => HandleRoundEntering(e, ctx) },
                { "Round_Processing", (e, ctx) => HandleRoundProcessing(e, ctx) },
                { "Round_Processed", (e, ctx) => HandleRoundProcessed(e, ctx) },
                { "Round_Exiting", (e, ctx) => HandleRoundExiting(e, ctx) },
                { "Round_Finished", (e, ctx) => HandleRoundFinished(e, ctx) },
                { "PlayerTurn_Entering", (e, ctx) => HandlePlayerTurnEntering(e, ctx) },
                { "PlayerTurn_Processing", (e, ctx) => HandlePlayerTurnProcessing(e, ctx) },
                { "PlayerTurn_Processed", (e, ctx) => HandlePlayerTurnProcessed(e, ctx) },
                { "PlayerTurn_Exiting", (e, ctx) => HandlePlayerTurnExiting(e, ctx) },
                { "PlayerTurn_Finished", (e, ctx) => HandlePlayerTurnFinished(e, ctx) },
                { "Praparation_Entering", (e, ctx) => HandlePraparationEntering(e, ctx) },
                { "Praparation_Processing", (e, ctx) => HandlePraparationProcessing(e, ctx) },
                { "Praparation_Processed", (e, ctx) => HandlePraparationProcessed(e, ctx) },
                { "Praparation_Exiting", (e, ctx) => HandlePraparationExiting(e, ctx) },
                { "Praparation_Finished", (e, ctx) => HandlePraparationFinished(e, ctx) },
                { "Judging_Entering", (e, ctx) => HandleJudgingEntering(e, ctx) },
                { "Judging_Processing", (e, ctx) => HandleJudgingProcessing(e, ctx) },
                { "Judging_Processed", (e, ctx) => HandleJudgingProcessed(e, ctx) },
                { "Judging_Exiting", (e, ctx) => HandleJudgingExiting(e, ctx) },
                { "Judging_Finished", (e, ctx) => HandleJudgingFinished(e, ctx) },
                { "Draw_Entering", (e, ctx) => HandleDrawEntering(e, ctx) },
                { "Draw_Processing", (e, ctx) => HandleDrawProcessing(e, ctx) },
                { "Draw_Processed", (e, ctx) => HandleDrawProcessed(e, ctx) },
                { "Draw_Exiting", (e, ctx) => HandleDrawExiting(e, ctx) },
                { "Draw_Finished", (e, ctx) => HandleDrawFinished(e, ctx) },
                { "Play_Entering", (e, ctx) => HandlePlayEntering(e, ctx) },
                { "Play_Processing", (e, ctx) => HandlePlayProcessing(e, ctx) },
                { "Play_Processed", (e, ctx) => HandlePlayProcessed(e, ctx) },
                { "Play_Exiting", (e, ctx) => HandlePlayExiting(e, ctx) },
                { "Play_Finished", (e, ctx) => HandlePlayFinished(e, ctx) },
                { "Discard_Entering", (e, ctx) => HandleDiscardEntering(e, ctx) },
                { "Discard_Processing", (e, ctx) => HandleDiscardProcessing(e, ctx) },
                { "Discard_Processed", (e, ctx) => HandleDiscardProcessed(e, ctx) },
                { "Discard_Exiting", (e, ctx) => HandleDiscardExiting(e, ctx) },
                { "Discard_Finished", (e, ctx) => HandleDiscardFinished(e, ctx) },
                { "End_Entering", (e, ctx) => HandleEndEntering(e, ctx) },
                { "End_Processing", (e, ctx) => HandleEndProcessing(e, ctx) },
                { "End_Processed", (e, ctx) => HandleEndProcessed(e, ctx) },
                { "End_Exiting", (e, ctx) => HandleEndExiting(e, ctx) },
                { "End_Finished", (e, ctx) => HandleEndFinished(e, ctx) }
            };
        }

        private void HandleGameEntering(Event gameEvent, GameContext context)
        {
            if (context.Players.Count == 0)
            {
                gameEvent.Stop();
                return;
            }

            // foreach (var player in context.Players)
            // {
            //     for (int i = 0; i < 4; i++)
            //     {
            //         Card card = context.Piles.GetCard("Draw", c => true);
            //         if (card != null)
            //         {
            //             player.AddToHand(card);
            //         }
            //     }
            // }
        }

        private void HandleGameProcessing(Event gameEvent, GameContext context)
        {
            Event roundEvent = new Event("Round");
            context.CreateEvent(roundEvent);
        }

        private void HandleRoundEntering(Event gameEvent, GameContext context)
        {
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

        private void HandleGameProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandleGameExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandleRoundProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandleRoundExiting(Event gameEvent, GameContext context)
        {
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
                    context.CreateEvent(phaseEvent);
                }
            }
        }

        private void HandlePlayerTurnProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayerTurnExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandleGameFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandleRoundFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayerTurnFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandlePraparationEntering(Event gameEvent, GameContext context)
        {
        }

        private void HandlePraparationProcessing(Event gameEvent, GameContext context)
        {
        }

        private void HandlePraparationProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandlePraparationExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandlePraparationFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandleJudgingEntering(Event gameEvent, GameContext context)
        {
        }

        private void HandleJudgingProcessing(Event gameEvent, GameContext context)
        {
        }

        private void HandleJudgingProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandleJudgingExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandleJudgingFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandleDrawEntering(Event gameEvent, GameContext context)
        {
        }

        private void HandleDrawProcessing(Event gameEvent, GameContext context)
        {
        }

        private void HandleDrawProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandleDrawExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandleDrawFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayEntering(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayProcessing(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandlePlayFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandleDiscardEntering(Event gameEvent, GameContext context)
        {
        }

        private void HandleDiscardProcessing(Event gameEvent, GameContext context)
        {
        }

        private void HandleDiscardProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandleDiscardExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandleDiscardFinished(Event gameEvent, GameContext context)
        {
        }

        private void HandleEndEntering(Event gameEvent, GameContext context)
        {
        }

        private void HandleEndProcessing(Event gameEvent, GameContext context)
        {
        }

        private void HandleEndProcessed(Event gameEvent, GameContext context)
        {
        }

        private void HandleEndExiting(Event gameEvent, GameContext context)
        {
        }

        private void HandleEndFinished(Event gameEvent, GameContext context)
        {
        }

        public override void Initialize(GameContext context)
        {
            this.context = context;
            Console.WriteLine("Initializing SGS ruleset");
            Console.WriteLine("   Setting up game with " + context.Players.Count + " players");
        }

        public override void EventHandler(Event gameEvent, GameContext context)
        {
            Console.WriteLine("Handling events for SGS ruleset");
            Console.WriteLine($"   Event: {gameEvent.Type}, Process: {gameEvent.Process}");

            string handlerKey = $"{gameEvent.Type}_{gameEvent.Process}";
            if (eventHandlers.ContainsKey(handlerKey))
            {
                eventHandlers[handlerKey](gameEvent, context);
            }
        }

        private void Pipeline(GameContext context)
        {
            Console.WriteLine("Processing game pipeline for SGS ruleset");
            Console.WriteLine("   Executing game phases and turns");
        }

        public override void GameStart(GameContext context)
        {
            for (int i = 0; i < 7; i++)
            {
                Player player = new Player(4, i, $"Player{i + 1}");
                context.Players.Add(player);
            }

            Event gameEvent = new Event("Game");
            context.CreateEvent(gameEvent);
        }
    }
}