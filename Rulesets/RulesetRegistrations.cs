using Shayou.Core.Domain.Entities;
using Shayou.Infrastructure.Interaction.Validation;
using System;
using System.Collections.Generic;

namespace Shayou.Rulesets
{
    public class RulesetRegistrations
    {
        public Dictionary<string, UiChoiceValidator> UiChoiceValidators { get; init; }
            = new Dictionary<string, UiChoiceValidator>();

        public Dictionary<string, List<Action<BaseEvent>>> TimingRuleActions { get; init; }
            = new Dictionary<string, List<Action<BaseEvent>>>();

        public Dictionary<string, Action<BaseEvent>> EventCallbacks { get; init; }
            = new Dictionary<string, Action<BaseEvent>>();
    }
}
