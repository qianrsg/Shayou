using Bang.Core.Domain.Entities;
using Bang.Core.Domain.Models;
using Bang.Rulesets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bang.Rulesets.ThreeKingdoms
{
    public class SGSRuleset : BaseRuleset
    {
        private readonly List<string> _playerTurnPhases;
        private Dictionary<string, Action<BaseEvent>> _eventHandlers;
        private Dictionary<string, Action<BaseEvent>> _eventCallbacks;

        public SGSRuleset() : base("sgs")
        {
            _playerTurnPhases = new List<string>
            {
                "PreparationPhase",
                "JudgingPhase",
                "DrawPhase",
                "PlayPhase",
                "DiscardPhase",
                "EndPhase"
            };

            InitializeEventHandlers();
            InitializeEventCallbacks();
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing SGS ruleset");
            Console.WriteLine($"   Setting up game with {Context.GetPlayers().Count} players");
        }

        public override void GameStart()
        {
            Context.CreateArea("Public_Deck");
            Context.CreateArea("Public_Discard");

            for (int i = 0; i < 8; i++)
            {
                var player = new Player(4, i, $"Player{i + 1}")
                {
                    Context = Context,
                    DeckName = "Public",
                    MaxHealth = 4,
                    Health = 4
                };
                player.InitializeAreas();

                Context.GetPlayers().Add(player);
            }

            var gameEvent = new Event("Game");
            Context.CreateEvent(gameEvent);
        }

        private void InitializeEventHandlers()
        {
            _eventHandlers = new Dictionary<string, Action<BaseEvent>>
            {
                { "Game_Entering", HandleGameEntering },
                { "PlayerTurn_Entering", HandlePlayerTurnEntering }
            };
        }

        private void InitializeEventCallbacks()
        {
            _eventCallbacks = new Dictionary<string, Action<BaseEvent>>
            {
                { "Game", OnGameCallback },
                { "Round", OnRoundCallback },
                { "PlayerTurn", OnPlayerTurnCallback },
                { "DrawPhase", OnDrawPhaseCallback },
                { "Draw", OnDrawCallback }
            };
        }

        public override void PrepareEvent(BaseEvent gameEvent)
        {
            if (_eventCallbacks.TryGetValue(gameEvent.Name, out var callback))
            {
                gameEvent.Callback = callback;
            }
        }

        public override void EventHandler(BaseEvent gameEvent)
        {
            string indent = new string(' ', Engine.StackDepth * 2);

            Console.WriteLine(
                $"{indent}Event: {gameEvent.Name}, Timing: {gameEvent.Timing}");

            string handlerKey = $"{gameEvent.Name}_{gameEvent.Timing}";

            if (_eventHandlers.TryGetValue(handlerKey, out var handler))
            {
                Thread.Sleep(10);
                handler(gameEvent);
            }
        }

        private void OnGameCallback(BaseEvent e)
        {
            var roundEvent = new Event("Round");
            Context.CreateEvent(roundEvent);
        }

        private void OnRoundCallback(BaseEvent e)
        {
            int newRound = Context.GetRound() + 1;
            Context.SetRound(newRound);

            var players = Context.GetPlayers();
            int playerCount = players.Count;

            Player currentPlayer = players
                .FirstOrDefault(p => p.Position == newRound % playerCount);

            if (currentPlayer == null)
                return;

            Context.SetCurrentPlayer(currentPlayer);

            var playerTurnEvent = new Event("PlayerTurn");
            Context.CreateEvent(playerTurnEvent);
        }

        private void OnPlayerTurnCallback(BaseEvent pe)
        {
            Player currentPlayer = Context.GetCurrentPlayer();

            if (currentPlayer?.TurnPhase == null)
                return;

            while (currentPlayer.TurnPhase.Count > 0)
            {
                string currentPhase = currentPlayer.TurnPhase[0];
                currentPlayer.TurnPhase.RemoveAt(0);

                var phaseEvent = new Event(currentPhase);

                if (currentPhase == "DrawPhase")
                {
                    phaseEvent.Num = 2;
                }

                Context.CreateEvent(phaseEvent);
            }
        }

        private void OnDrawPhaseCallback(BaseEvent de)
        {
            Player currentPlayer = Context.GetCurrentPlayer();

            if (currentPlayer != null && de.Num > 0)
            {
                currentPlayer.Draw(de.Num);
            }
        }

        private void OnDrawCallback(BaseEvent de)
        {
            Player player = de.SourcePlayer;
            if (player == null || de.Num <= 0)
                return;

            string deckName = player.DeckName;
            List<Card> cards = Context.DrawTopCard($"{deckName}_Deck", de.Num);
            List<Card> deck = Context.GetArea($"{deckName}_Deck");

            Console.WriteLine($"[{player.HeroName}] 摸牌 {cards.Count} 张:");
            foreach (var card in cards)
            {
                Console.WriteLine($"  - {card.Suit}{card.Rank} {card.Name}");
            }

            Engine.MoveCards(cards, deck, player.GetHand());
        }

        private void HandleGameEntering(BaseEvent gameEvent)
        {
            Context.LoadCardsFromJson(
                "Config/Piles/standard.json",
                "Public_Deck");

            Context.Shuffle("Public_Deck");

            foreach (var player in Context.GetPlayers())
            {
                player.Draw(4);
            }
        }

        private void HandlePlayerTurnEntering(BaseEvent gameEvent)
        {
            Player currentPlayer = Context.GetCurrentPlayer();
            if (currentPlayer == null)
                return;

            currentPlayer.TurnPhase.Clear();

            foreach (string phase in _playerTurnPhases)
            {
                currentPlayer.TurnPhase.Add(phase);
            }
        }
    }
}
