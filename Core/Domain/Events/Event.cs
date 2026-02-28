namespace Bang.Core.Domain.Entities
{
    public class Event : BaseEvent
    {
        public Event()
        {
            Name = "";
            SourceCard = null;
            Cards = null;
            SourcePlayer = null;
            TargetPlayer = null;
            SourceContainer = null;
            TargetContainer = null;
            Data = new Dictionary<string, object>();
            Num = 0;
            Timing = "Entering";
        }

        public Event(string name)
        {
            Name = name;
            SourceCard = null;
            Cards = null;
            SourcePlayer = null;
            TargetPlayer = null;
            SourceContainer = null;
            TargetContainer = null;
            Data = new Dictionary<string, object>();
            Num = 0;
            Timing = "Entering";
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