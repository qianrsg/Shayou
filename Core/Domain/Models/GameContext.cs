using Bang.Core.Domain.Entities;

namespace Bang.Core.Domain.Models
{
    public class GameContext
    {
        public List<Player> Players { get; set; }
        public Pile Piles { get; set; }
        public int CurrentTurn { get; set; }
        public string CurrentPhase { get; set; }

        public GameContext()
        {
            Players = new List<Player>();
            Piles = new Pile();
            CurrentTurn = 0;
            CurrentPhase = "";
        }
    }
}