using Shayou.Engine.Core.Domain.Entities;
using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Core.StateMachine;

namespace Shayou.Engine.Core.Domain.Models
{
    public class GameContext
    {
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        public Zone Zone { get; set; }
        public GameRegistry Registry { get; set; }
        public GameEngine Engine { get; set; }
        public int Round { get; set; }

        public GameContext()
        {
            Players = new List<Player>();
            Zone = new Zone();
            Registry = new GameRegistry();
            Round = 0;
            CurrentPlayer = null;
        }

        public void CreateEvent(BaseEvent gameEvent)
        {
            Engine.CreateEvent(gameEvent);
        }
    }
}
