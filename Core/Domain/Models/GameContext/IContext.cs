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
    }
}
