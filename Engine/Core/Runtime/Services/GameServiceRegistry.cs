using Shayou.Engine.Core.Runtime.Input;
using Shayou.Engine.Foundations.Input;

namespace Shayou.Engine.Core.Runtime.Services
{
    public class GameServiceRegistry
    {
        public GameEngine EventDispatcher { get; }
        public IInputService InputService { get; }

        public GameServiceRegistry(
            GameEngine eventDispatcher,
            IInputService inputService)
        {
            EventDispatcher = eventDispatcher;
            InputService = inputService;
        }
    }
}
