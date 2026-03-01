using Shayou.Core.Domain.Entities;
using Shayou.Core.StateMachine;

namespace Shayou.Core.Domain.Models
{
    public class GameContext
    {
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        public Zone Zone { get; set; }
        public GameEngine Engine { get; set; }
        public int Round { get; set; }

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
    }
}
