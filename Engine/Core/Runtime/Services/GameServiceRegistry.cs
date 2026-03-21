using Shayou.Engine.Core.Runtime.Input;

namespace Shayou.Engine.Core.Runtime.Services
{
    public class GameServiceRegistry
    {
        public GameEngine EventDispatcher { get; }
        public InputManager InputService { get; }

        public GameServiceRegistry(
            GameEngine eventDispatcher, 
            InputManager inputService)
        {
            EventDispatcher = eventDispatcher;
            InputService = inputService;
        }
    }
}
