using Shayou.Engine.Core.Domain.Entities;
using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Foundations.Events;
using Shayou.Engine.Foundations.Input;
using Shayou.Engine.Foundations.Rulesets;
using System;
using System.Collections.Generic;

namespace Shayou.Gameplay.Rulesets.ThreeKingdoms
{
    public class SGSRuleset : BaseRuleset
    {
        private readonly List<string> _playerTurnPhases;

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
        }

        public override void Initialize()
        {
            Console.WriteLine("Initializing SGS ruleset");
            Console.WriteLine($"   Setting up game with {Context.Players.Count} players");
        }

        public override RulesetRegistrations GetRegistrations()
        {
            return new RulesetRegistrations
            {
                TimingRuleActions = new Dictionary<string, List<Action<BaseEvent>>>
                {
                    { "Game_Entering", new List<Action<BaseEvent>> { HandleGameEntering } },
                    { "PlayerTurn_Entering", new List<Action<BaseEvent>> { HandlePlayerTurnEntering } }
                },
                EventCallbacks = new Dictionary<string, Action<BaseEvent>>
                {
                    { "Game", OnGameCallback },
                    { "Round", OnRoundCallback },
                    { "PlayerTurn", OnPlayerTurnCallback },
                    { "DrawPhase", OnDrawPhaseCallback },
                    { "PlayPhase", OnPlayPhaseCallback },
                    { "Draw", OnDrawCallback }
                },
                InputRequests = new Dictionary<string, Func<InputRequest>>
                {
                    { "game.PlayPhase", CreatePlayPhaseRequest }
                }
            };
        }

        public override void SetupGame()
        {
            Context.Cards.CreateArea("Public_Deck");
            Context.Cards.CreateArea("Public_Discard");

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

                Context.Players.Add(player);
            }
        }

        private void OnGameCallback(BaseEvent e)
        {
            var roundEvent = new Event("Round");
            EmitEvent(roundEvent);
        }

        private void OnRoundCallback(BaseEvent e)
        {
            int newRound = Context.Round + 1;
            Context.Round = newRound;

            var players = Context.Players;
            int playerCount = players.Count;

            Player? currentPlayer = players.GetByPosition(newRound % playerCount);

            if (currentPlayer == null)
                return;

            Context.Players.SetCurrentPlayer(currentPlayer);

            var playerTurnEvent = new Event("PlayerTurn");
            EmitEvent(playerTurnEvent);
        }

        private void OnPlayerTurnCallback(BaseEvent pe)
        {
            Player? currentPlayer = Context.Players.CurrentPlayer;

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

                EmitEvent(phaseEvent);
            }
        }

        private void OnDrawPhaseCallback(BaseEvent de)
        {
            Player? currentPlayer = Context.Players.CurrentPlayer;

            if (currentPlayer != null && de.Num > 0)
            {
                currentPlayer.Draw(de.Num);
            }
        }

        private void OnDrawCallback(BaseEvent de)
        {
            Player? player = de.SourcePlayer;
            if (player == null || de.Num <= 0)
                return;

            string deckName = player.DeckName;
            List<Card> cards = Context.Cards.DrawTopCard($"{deckName}_Deck", de.Num);
            List<Card> hand = player.GetHand();

            Console.WriteLine($"[{player.HeroName}] drew {cards.Count} cards");
            foreach (var card in cards)
            {
                Console.WriteLine($"  - {card.Suit}{card.Rank} {card.Name}");
                hand.Add(card);
            }
        }

        private void HandleGameEntering(BaseEvent gameEvent)
        {
            Context.Cards.LoadFromJson(
                "Config/Piles/standard.json",
                "Public_Deck");

            Context.Cards.Shuffle("Public_Deck");

            foreach (var player in Context.Players)
            {
                player.Draw(4);
            }
        }

        private void HandlePlayerTurnEntering(BaseEvent gameEvent)
        {
            Player? currentPlayer = Context.Players.CurrentPlayer;
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
            Player? currentPlayer = Context.Players.CurrentPlayer;
            if (currentPlayer == null)
                return;

            while (true)
            {
                InputSubmission submission = RequestInput("game.PlayPhase");
                Console.WriteLine($"[Backend] Received input: {submission.ActionKey}");

                if (submission.ActionKey == "game.Pass")
                {
                    break;
                }
            }
        }

        private InputRequest CreatePlayPhaseRequest()
        {
            return new ActionInputRequest(
                "game.PlayPhase",
                new[] { "game.Pass", "game.Confirm", "game.Cancel" });
        }
    }
}
