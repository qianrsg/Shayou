using Bang.Core.Domain.Entities;
using Bang.Core.StateMachine;

namespace Bang.Core.Domain.Models
{
    public class GameContext : IContext
    {
        public List<Player> Players { get; set; }
        public Zone Zone { get; set; }
        public int Round { get; set; }
        public Player CurrentPlayer { get; set; }
        public GameEngine Engine { get; set; }

        public GameContext()
        {
            Players = new List<Player>();
            Zone = new Zone();
            Round = 0;
            CurrentPlayer = null;
        }

        public void CreateEvent(BaseEvent gameEvent)
        {
            Engine.CreateEvent(gameEvent);
        }

        public int GetRound()
        {
            return Round;
        }

        public void SetRound(int round)
        {
            Round = round;
        }

        public Player GetCurrentPlayer()
        {
            return CurrentPlayer;
        }

        public void SetCurrentPlayer(Player player)
        {
            CurrentPlayer = player;
        }

        public List<Player> GetPlayers()
        {
            return Players;
        }

        public List<Card> CreateArea(string areaName)
        {
            return Zone.CreateArea(areaName);
        }

        public void AddCard(string areaName, Card card)
        {
            Zone.AddCard(areaName, card);
        }

        public void AddCard(string areaName, IEnumerable<Card> cards)
        {
            Zone.AddCard(areaName, cards);
        }

        public void RemoveCard(string areaName, Card card)
        {
            Zone.RemoveCard(areaName, card);
        }

        public void RemoveCard(string areaName, IEnumerable<Card> cards)
        {
            Zone.RemoveCard(areaName, cards);
        }

        public void Shuffle(string areaName)
        {
            Zone.Shuffle(areaName);
        }

        public List<Card> GetTopCard(string areaName, int count = 1)
        {
            return Zone.GetTopCard(areaName, count);
        }

        public List<Card> GetBottomCard(string areaName, int count = 1)
        {
            return Zone.GetBottomCard(areaName, count);
        }

        public List<Card> DrawTopCard(string areaName, int count = 1)
        {
            return Zone.DrawTopCard(areaName, count);
        }

        public List<Card> DrawBottomCard(string areaName, int count = 1)
        {
            return Zone.DrawBottomCard(areaName, count);
        }

        public List<Card> GetArea(string areaName)
        {
            return Zone.GetArea(areaName);
        }

        public void LoadCardsFromJson(string filePath, string areaName)
        {
            Zone.LoadFromJson(filePath, areaName);
        }
    }
}
