using Shayou.Core.Domain.Entities;
using Shayou.Infrastructure.Interaction;
using Shayou.Infrastructure.Interaction.Contracts;
using Shayou.Infrastructure.Interaction.Validation;
using Shayou.Rulesets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shayou.Rulesets.ThreeKingdoms
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
            Console.WriteLine($"   Setting up game with {Engine.Context.Players.Count} players");
        }

        public override RulesetRegistrations GetRegistrations()
        {
            return new RulesetRegistrations
            {
                UiChoiceValidators = new Dictionary<string, UiChoiceValidator>
                {
                    { "PlayPhase", new UiChoiceValidator("PlayPhase") }
                },
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
                }
            };
        }

        public override void SetupGame()
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
        }

        private InputRequestPacket CreateInputRequestPacket()
        {
            return new InputRequestPacket
            {
                WindowId = "",
                PlayerId = "",
                RequestKey = "",
                PromptKey = "",
                ValidatorKey = null,
                TimeoutMs = 10000,
                CanBeCancelled = true
            };
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
            List<Card> hand = player.GetHand();

            Console.WriteLine($"[{player.HeroName}] 摸牌 {cards.Count} 张:");
            foreach (var card in cards)
            {
                Console.WriteLine($"  - {card.Suit}{card.Rank} {card.Name}");
                hand.Add(card);
            }
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
            Player currentPlayer = Engine.Context.CurrentPlayer;
            if (currentPlayer == null)
                return;

            const string uiChoiceValidatorKey = "PlayPhase";

            while (true)
            {
                var requestPacket = CreateInputRequestPacket() with
                {
                    WindowId = "PlayPhase",
                    PlayerId = currentPlayer.Position.ToString(),
                    RequestKey = "PlayPhase",
                    PromptKey = "prompt.play_phase",
                    ValidatorKey = uiChoiceValidatorKey
                };

                ConsoleLogger.Log(LogChannel.Backend, $"发送输入请求: {requestPacket.RequestKey}");
                string input = Engine.WaitForInput(requestPacket);
                ConsoleLogger.Log(LogChannel.Backend, $"收到输入: {input}");
                if (input == "pass")
                    break;
            }
        }
    }
}
