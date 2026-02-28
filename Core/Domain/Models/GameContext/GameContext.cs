using Bang.Core.Domain.Entities;
using Bang.Core.StateMachine;

namespace Bang.Core.Domain.Models
{
    public class GameContext : IContext
    {
        public List<Player> Players { get; set; }
        public Pile Piles { get; set; }
        public int Round { get; set; }
        public Player CurrentPlayer { get; set; }
        public GameEngine Engine { get; set; }

        public GameContext()
        {
            Players = new List<Player>();
            Piles = new Pile();
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
    }
}
