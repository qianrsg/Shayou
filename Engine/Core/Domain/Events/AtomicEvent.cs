using Shayou.Engine.Foundations.Events;
using Shayou.Engine.Core.Domain.Entities;

namespace Shayou.Engine.Core.Domain.Events
{
    public class AtomicEvent : BaseEvent
    {
        public AtomicEvent()
        {
        }

        public AtomicEvent(string name) : base(name)
        {
        }

        public override void AdvanceProcess()
        {
            if (Timing == "Entering")
            {
                Callback?.Invoke(this);
                Timing = "Finished";
            }
            else
            {
                Timing = "End";
            }
        }
    }
}
