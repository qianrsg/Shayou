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
        private Dictionary<string, Action<BaseEvent, GameContext>> _eventHandlers;
        private IContext _context;

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
        }

        // ---------- 游戏初始化 ----------
        public override void Initialize(GameContext context)
        {
            _context = context;

            Console.WriteLine("Initializing SGS ruleset");
            Console.WriteLine($"   Setting up game with {context.Players.Count} players");
        }

        public override void GameStart(GameContext context)
        {
            for (int i = 0; i < 8; i++)
            {
                var player = new Player(4, i, $"Player{i + 1}")
                {
                    Context = context,
                    Pile = "Public",
                    MaxHealth = 4,
                    Health = 4
                };

                context.Players.Add(player);
            }

            var gameEvent = new Event("Game")
            {
                Callback = OnGameCallback
            };

            context.CreateEvent(gameEvent);
        }

        private void InitializeEventHandlers()
        {
            _eventHandlers = new Dictionary<string, Action<BaseEvent, GameContext>>
            {
                { "Game_Entering", HandleGameEntering },
                { "PlayerTurn_Entering", HandlePlayerTurnEntering }
            };
        }

        public override void EventHandler(BaseEvent gameEvent, GameContext context)
        {
            string indent = new string(' ', context.Engine.StackDepth * 2);

            Console.WriteLine(
                $"{indent}Event: {gameEvent.Name}, Process: {gameEvent.Process}");

            string handlerKey = $"{gameEvent.Name}_{gameEvent.Process}";

            if (_eventHandlers.TryGetValue(handlerKey, out var handler))
            {
                Thread.Sleep(10);
                handler(gameEvent, context);
            }
        }

        // ---------- 游戏流程回调 ----------
        private void OnGameCallback(BaseEvent e)
        {
            var roundEvent = new Event("Round")
            {
                Callback = OnRoundCallback
            };

            _context.CreateEvent(roundEvent);
        }

        private void OnRoundCallback(BaseEvent e)
        {
            int newRound = _context.GetRound() + 1;
            _context.SetRound(newRound);

            var players = _context.GetPlayers();
            int playerCount = players.Count;

            Player currentPlayer = players
                .FirstOrDefault(p => p.Position == newRound % playerCount);

            if (currentPlayer == null)
                return;

            _context.SetCurrentPlayer(currentPlayer);

            var playerTurnEvent = new Event("PlayerTurn")
            {
                Callback = OnPlayerTurnCallback
            };

            _context.CreateEvent(playerTurnEvent);
        }

        private void OnPlayerTurnCallback(BaseEvent pe)
        {
            Player currentPlayer = _context.GetCurrentPlayer();

            if (currentPlayer?.TurnPhase == null)
                return;

            while (currentPlayer.TurnPhase.Count > 0)
            {
                string currentPhase = currentPlayer.TurnPhase[0];
                currentPlayer.TurnPhase.RemoveAt(0);

                var phaseEvent = new Event(currentPhase);

                if (currentPhase == "Draw")
                {
                    phaseEvent.Num = 2;
                    phaseEvent.Callback = OnDrawCallback;
                }

                _context.CreateEvent(phaseEvent);
            }
        }

        private void OnDrawCallback(BaseEvent de)
        {
            Player currentPlayer = _context.GetCurrentPlayer();

            if (currentPlayer != null && de.Num > 0)
            {
                currentPlayer.Draw(de.Num);
            }
        }

        // ---------- 事件处理器 ----------
        private void HandleGameEntering(BaseEvent gameEvent, GameContext context)
        {
            context.Piles.CreatePilePair("Public");

            context.Piles.LoadFromJson(
                "Config/Piles/standard.json",
                "Public_Deck");

            context.Piles.Shuffle("Public_Deck");

            foreach (var player in context.Players)
            {
                player.Draw(4);
            }
        }

        private void HandlePlayerTurnEntering(
            BaseEvent gameEvent,
            GameContext context)
        {
            if (context.CurrentPlayer == null)
                return;

            context.CurrentPlayer.TurnPhase.Clear();

            foreach (string phase in _playerTurnPhases)
            {
                context.CurrentPlayer.TurnPhase.Add(phase);
            }
        }
    }
}
