using Shayou.Engine.Foundations.Events;
using Shayou.Engine.Foundations.Input;
using Shayou.Engine.Foundations.Rulesets;
using Shayou.Engine.Core.Domain.Entities;
using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Core.Runtime.Context;
using Shayou.Engine.Core.Runtime.Input;
using Shayou.Engine.Core.Runtime.Services;
using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;
using System.Collections.Generic;
using System.Threading;

namespace Shayou.Engine.Core.Runtime
{
        public class GameEngine
    {
        private GameContext Context { get; set; }
        private BaseRuleset Ruleset { get; set; } = null!;
        private IInputService InputManager { get; set; } = null!;
        public IClientConnection ClientConnection { get; private set; } = null!;
        private IServerConnection ServerConnection { get; set; } = null!;

        public GameEngine(
            BaseRuleset ruleset,
            IServerConnection serverConnection,
            IClientConnection clientConnection)
        {
            InitializeRuntime(serverConnection, clientConnection);
            Context = CreateContext();
            AttachRuleset(ruleset);
        }

        private GameContext CreateContext()
        {
            return new GameContext(new GameServiceRegistry(this, InputManager));
        }

        private void InitializeRuntime(
            IServerConnection serverConnection,
            IClientConnection clientConnection)
        {
            ServerConnection = serverConnection;
            ClientConnection = clientConnection;
            InputManager = new InputManager(ServerConnection);
        }

        private void AttachRuleset(BaseRuleset ruleset)
        {
            Ruleset = ruleset;
            Ruleset.Context = Context;
            RegisterRulesetRegistrations();
            Ruleset.Initialize();
        }

        private void RegisterRulesetRegistrations()
        {
            RulesetRegistrations registrations = Ruleset.GetRegistrations();

            foreach (var ruleActionPair in registrations.TimingRuleActions)
            {
                Context.Registry.TimingRuleActions.Register(
                    ruleActionPair.Key,
                    ruleActionPair.Value);
            }

            foreach (var callbackPair in registrations.EventCallbacks)
            {
                Context.Registry.EventCallbacks.Register(
                    callbackPair.Key,
                    callbackPair.Value);
            }

            foreach (var inputRequestPair in registrations.InputRequests)
            {
                Context.Registry.InputRequests.Register(
                    inputRequestPair.Key,
                    inputRequestPair.Value);
            }
        }

        private void ExecuteTimingRuleAction(BaseEvent gameEvent)
        {
            string ruleActionKey = $"{gameEvent.Name}_{gameEvent.Timing}";
            IReadOnlyList<Action<BaseEvent>> ruleActions = Context.Registry.TimingRuleActions.Get(ruleActionKey);

            if (ruleActions.Count == 0)
                return;

            foreach (var ruleAction in ruleActions)
            {
                ruleAction(gameEvent);
            }
        }

        private void AttachEventCallback(BaseEvent gameEvent)
        {
            Action<BaseEvent>? callback = Context.Registry.EventCallbacks.Get(gameEvent.Name);

            if (callback != null)
            {
                gameEvent.Callback = callback;
            }
        }

        private void SendPackets(IEnumerable<PacketEnvelope> packets)
        {
            List<PacketEnvelope> packetList = new List<PacketEnvelope>();

            foreach (PacketEnvelope packet in packets)
            {
                packetList.Add(packet);
            }

            if (packetList.Count == 0)
            {
                return;
            }

            if (packetList.Count == 1)
            {
                ServerConnection.SendPacket(packetList[0]);
                return;
            }

            ServerConnection.SendPacket(new PacketBatch
            {
                Messages = packetList
            });
        }

        private EventPacket CreateEventPacket(BaseEvent gameEvent)
        {
            return new EventPacket
            {
                Key = GetEventKey(gameEvent),
                SourceCard = FormatCard(gameEvent.SourceCard),
                Cards = GetCards(gameEvent),
                SourcePlayer = FormatPlayer(gameEvent.SourcePlayer),
                TargetPlayer = FormatPlayer(gameEvent.TargetPlayer),
                Num = GetNum(gameEvent),
                Players = GetPlayers(gameEvent),
                Nums = GetNums(gameEvent)
            };
        }

        private static string GetEventKey(BaseEvent gameEvent)
        {
            return $"game.{gameEvent.Name}";
        }

        private List<string> GetPlayers(BaseEvent gameEvent)
        {
            HashSet<string> players = new HashSet<string>();

            if (gameEvent.SourcePlayer != null)
            {
                players.Add(gameEvent.SourcePlayer.Position.ToString());
            }

            if (gameEvent.TargetPlayer != null)
            {
                players.Add(gameEvent.TargetPlayer.Position.ToString());
            }

            if (Context.Players.CurrentPlayer != null)
            {
                players.Add(Context.Players.CurrentPlayer.Position.ToString());
            }

            return players.ToList();
        }

        private static List<string> GetCards(BaseEvent gameEvent)
        {
            List<string> cards = new List<string>();

            if (gameEvent.Cards == null)
            {
                return cards;
            }

            foreach (Card card in gameEvent.Cards)
            {
                string? formattedCard = FormatCard(card);
                if (!string.IsNullOrWhiteSpace(formattedCard))
                {
                    cards.Add(formattedCard);
                }
            }

            return cards;
        }

        private static string? FormatCard(Card? card)
        {
            if (card == null)
            {
                return null;
            }

            return $"{card.Name}|{card.Suit}{card.Rank}|{card.Id}";
        }

        private static string? FormatPlayer(Player? player)
        {
            if (player == null)
            {
                return null;
            }

            return $"{player.Position}|{player.HeroName}|{player.Health}/{player.MaxHealth}";
        }

        private static string? GetNum(BaseEvent gameEvent)
        {
            return gameEvent.Num == 0 ? null : gameEvent.Num.ToString();
        }

        private static List<int> GetNums(BaseEvent gameEvent)
        {
            return gameEvent.Num == 0
                ? new List<int>()
                : new List<int> { gameEvent.Num };
        }

        public void DispatchEvent(BaseEvent gameEvent)
        {
            AttachEventCallback(gameEvent);

            while (!gameEvent.IsEnd())
            {
                if (gameEvent.Timing == "Processing")
                {
                    SendPackets(new[] { CreateEventPacket(gameEvent) });
                }

                ExecuteTimingRuleAction(gameEvent);
                gameEvent.AdvanceProcess();
            }
        }

        public void GameStart()
        {
            Thread gameThread = new Thread(() =>
            {
                Ruleset.SetupGame();
                DispatchEvent(new Event("Game"));
            });
            gameThread.IsBackground = true;
            gameThread.Start();
        }
    }
}
