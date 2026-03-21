using Shayou.Engine.Foundations.Events;
using System;
using System.Collections.Generic;

namespace Shayou.Engine.Foundations.Rulesets
{
    public class RulesetRegistrations
    {
        public Dictionary<string, List<Action<BaseEvent>>> TimingRuleActions { get; init; }
            = new Dictionary<string, List<Action<BaseEvent>>>();

        public Dictionary<string, Action<BaseEvent>> EventCallbacks { get; init; }
            = new Dictionary<string, Action<BaseEvent>>();
    }
}
