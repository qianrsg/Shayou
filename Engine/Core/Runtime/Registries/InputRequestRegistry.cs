using Shayou.Engine.Foundations.Input;

namespace Shayou.Engine.Core.Runtime.Registries
{
    public class InputRequestRegistry
    {
        private readonly Dictionary<string, Func<InputRequest>> requests;

        public InputRequestRegistry()
        {
            requests = new Dictionary<string, Func<InputRequest>>();
        }

        public void Register(string key, Func<InputRequest> requestFactory)
        {
            requests[key] = requestFactory;
        }

        public InputRequest Create(string key)
        {
            if (!requests.TryGetValue(key, out Func<InputRequest>? requestFactory))
            {
                throw new InvalidOperationException(
                    $"Input request '{key}' is not registered.");
            }

            return requestFactory();
        }
    }
}
