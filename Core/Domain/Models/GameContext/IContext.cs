using Bang.Core.Domain.Entities;

namespace Bang.Core.Domain.Models
{
    public interface IContext
    {
        void CreateEvent(BaseEvent gameEvent);
        int GetRound();
        void SetRound(int round);
        Player GetCurrentPlayer();
        void SetCurrentPlayer(Player player);
        List<Player> GetPlayers();
        List<Card> CreateArea(string areaName);
        void AddCard(string areaName, Card card);
        void AddCard(string areaName, IEnumerable<Card> cards);
        void RemoveCard(string areaName, Card card);
        void RemoveCard(string areaName, IEnumerable<Card> cards);
        void Shuffle(string areaName);
        List<Card> GetTopCard(string areaName, int count = 1);
        List<Card> GetBottomCard(string areaName, int count = 1);
        List<Card> DrawTopCard(string areaName, int count = 1);
        List<Card> DrawBottomCard(string areaName, int count = 1);
        List<Card> GetArea(string areaName);
        void LoadCardsFromJson(string filePath, string areaName);
    }
}
