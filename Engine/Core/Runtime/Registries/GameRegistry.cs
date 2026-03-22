using Shayou.Engine.Foundations.Events;
using System;
using System.Collections.Generic;

namespace Shayou.Engine.Core.Runtime.Registries
{
    public class GameRegistry
    {
        public TimingRuleActionRegistry TimingRuleActions { get; }
        public EventCallbackRegistry EventCallbacks { get; }
        public InputRequestRegistry InputRequests { get; }

        public GameRegistry()
        {
            TimingRuleActions = new TimingRuleActionRegistry();
            EventCallbacks = new EventCallbackRegistry();
            InputRequests = new InputRequestRegistry();
        }
    }

    public class TimingRuleActionRegistry
    {
        private readonly Dictionary<string, List<Action<BaseEvent>>> _actions;

        public TimingRuleActionRegistry()
        {
            _actions = new Dictionary<string, List<Action<BaseEvent>>>();
        }

        public void Register(string key, Action<BaseEvent> action)
        {
            if (!_actions.TryGetValue(key, out var actions))
            {
                actions = new List<Action<BaseEvent>>();
                _actions[key] = actions;
            }

            actions.Add(action);
        }

        public void Register(string key, IEnumerable<Action<BaseEvent>> actions)
        {
            foreach (var action in actions)
            {
                Register(key, action);
            }
        }

        public IReadOnlyList<Action<BaseEvent>> Get(string key)
        {
            return _actions.TryGetValue(key, out var actions)
                ? actions
                : Array.Empty<Action<BaseEvent>>();
        }
    }

    public class EventCallbackRegistry
    {
        private readonly Dictionary<string, Action<BaseEvent>> _callbacks;

        public EventCallbackRegistry()
        {
            _callbacks = new Dictionary<string, Action<BaseEvent>>();
        }

        public void Register(string key, Action<BaseEvent> callback)
        {
            _callbacks[key] = callback;
        }

        public Action<BaseEvent>? Get(string key)
        {
            return _callbacks.TryGetValue(key, out var callback)
                ? callback
                : null;
        }
    }

}
