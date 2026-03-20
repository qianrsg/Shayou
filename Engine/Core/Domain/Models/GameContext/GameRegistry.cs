using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Interaction.Validation;
using System;

namespace Shayou.Engine.Core.Domain.Models
{
    public class GameRegistry
    {
        public UiChoiceValidatorRegistry UiChoiceValidators { get; }
        public TimingRuleActionRegistry TimingRuleActions { get; }
        public EventCallbackRegistry EventCallbacks { get; }

        public GameRegistry()
        {
            UiChoiceValidators = new UiChoiceValidatorRegistry();
            TimingRuleActions = new TimingRuleActionRegistry();
            EventCallbacks = new EventCallbackRegistry();
        }
    }

    public class UiChoiceValidatorRegistry
    {
        private readonly Dictionary<string, UiChoiceValidator> _validators;

        public UiChoiceValidatorRegistry()
        {
            _validators = new Dictionary<string, UiChoiceValidator>();
        }

        public void Register(UiChoiceValidator validator)
        {
            _validators[validator.Key] = validator;
        }

        public void Register(string key, UiChoiceValidator validator)
        {
            _validators[key] = validator;
        }

        public UiChoiceValidator? Get(string key)
        {
            return _validators.TryGetValue(key, out var validator)
                ? validator
                : null;
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
