using Shayou.Engine.Core.Runtime.Registries;
using Shayou.Engine.Core.Runtime.Services;

namespace Shayou.Engine.Core.Runtime.Context
{
    public class GameContext
    {
        public PlayerManager Players { get; }
        public CardManager Cards { get; }
        public GameServiceRegistry Services { get; }
        public GameRegistry Registry { get; }
        public int Round { get; set; }

        public GameContext(GameServiceRegistry services)
        {
            Players = new PlayerManager();
            Cards = new CardManager();
            Services = services;
            Registry = new GameRegistry();
            Round = 0;
        }
    }
}
