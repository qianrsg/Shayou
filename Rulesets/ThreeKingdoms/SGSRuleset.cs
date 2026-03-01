using Shayou.Core.Domain.Entities;
using Shayou.Rulesets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shayou.Rulesets.ThreeKingdoms
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
            Console.WriteLine($"   Setting up game with {Engine.Context.Players.Count} players");
        }

        public override void GameStart()
        {
            Engine.Context.Zone.CreateArea("Public_Deck");
            Engine.Context.Zone.CreateArea("Public_Discard");

            for (int i = 0; i < 8; i++)
            {
                var player = new Player(4, i, $"Player{i + 1}")
                {
                    Context = Engine.Context,
                    DeckName = "Public",
                    MaxHealth = 4,
                    Health = 4
                };
                player.InitializeAreas();

                Engine.Context.Players.Add(player);
            }

            var gameEvent = new Event("Game");
            Engine.Context.CreateEvent(gameEvent);
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
                { "PlayPhase", OnPlayPhaseCallback },
                { "Draw", OnDrawCallback },
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
            Engine.Context.CreateEvent(roundEvent);
        }

        private void OnRoundCallback(BaseEvent e)
        {
            int newRound = Engine.Context.Round + 1;
            Engine.Context.Round = newRound;

            var players = Engine.Context.Players;
            int playerCount = players.Count;

            Player currentPlayer = players
                .FirstOrDefault(p => p.Position == newRound % playerCount);

            if (currentPlayer == null)
                return;

            Engine.Context.CurrentPlayer = currentPlayer;

            var playerTurnEvent = new Event("PlayerTurn");
            Engine.Context.CreateEvent(playerTurnEvent);
        }

        private void OnPlayerTurnCallback(BaseEvent pe)
        {
            Player currentPlayer = Engine.Context.CurrentPlayer;

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

                Engine.Context.CreateEvent(phaseEvent);
            }
        }

        private void OnDrawPhaseCallback(BaseEvent de)
        {
            Player currentPlayer = Engine.Context.CurrentPlayer;

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
            List<Card> cards = Engine.Context.Zone.DrawTopCard($"{deckName}_Deck", de.Num);
            List<Card> deck = Engine.Context.Zone.GetArea($"{deckName}_Deck");

            Console.WriteLine($"[{player.HeroName}] 摸牌 {cards.Count} 张:");
            foreach (var card in cards)
            {
                Console.WriteLine($"  - {card.Suit}{card.Rank} {card.Name}");
            }

            Engine.MoveCard(cards, deck, player.GetHand());
        }

        private void HandleGameEntering(BaseEvent gameEvent)
        {
            Engine.Context.Zone.LoadFromJson(
                "Config/Piles/standard.json",
                "Public_Deck");

            Engine.Context.Zone.Shuffle("Public_Deck");

            foreach (var player in Engine.Context.Players)
            {
                player.Draw(4);
            }
        }

        private void HandlePlayerTurnEntering(BaseEvent gameEvent)
        {
            Player currentPlayer = Engine.Context.CurrentPlayer;
            if (currentPlayer == null)
                return;

            currentPlayer.TurnPhase.Clear();

            foreach (string phase in _playerTurnPhases)
            {
                currentPlayer.TurnPhase.Add(phase);
            }
        }

        private void OnPlayPhaseCallback(BaseEvent e)
        {
            while (true) {
                Console.WriteLine("等待玩家输入，按任意键继续...");
                string input = Engine.WaitForInput();
                Console.WriteLine($"收到输入: {input}");
                if (input == "pass") break;
            }
        }
    }
}
