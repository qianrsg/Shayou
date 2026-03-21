using Shayou.Engine.Foundations.Events;
using Shayou.Engine.Core.Domain.Entities;

namespace Shayou.Engine.Core.Domain.Events
{
    public class Event : BaseEvent
    {
        public Event()
        {
        }

        public Event(string name) : base(name)
        {
        }

        public override void AdvanceProcess()
        {
            if (Timing == "Entering")
            {
                Timing = "Processing";
            }
            else if (Timing == "Processing")
            {
                Callback?.Invoke(this);
                Timing = "Processed";
            }
            else if (Timing == "Processed")
            {
                Timing = "Finished";
            }
            else if (Timing == "Finished")
            {
                Timing = "Exiting";
            }
            else 
            {
                Timing = "End";
            }
        }
    }
}
