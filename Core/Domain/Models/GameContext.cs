using Bang.Core.Domain.Entities;
using Bang.Core.StateMachine;

namespace Bang.Core.Domain.Models
{
    public class GameContext
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

        public void CreateEvent(Event gameEvent)
        {
            Engine.CreateEvent(gameEvent);
        }
    }
}