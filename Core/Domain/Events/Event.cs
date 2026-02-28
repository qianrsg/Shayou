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
            Process = "Entering";
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
            Process = "Entering";
        }

        public override void AdvanceProcess()
        {
            if (Process == "Entering")
            {
                Process = "Processing";
            }
            else if (Process == "Processing")
            {
                Callback?.Invoke(this);
                Process = "Processed";
            }
            else if (Process == "Processed")
            {
                Process = "Finished";
            }
            else if (Process == "Finished")
            {
                Process = "Exiting";
            }
            else 
            {
                Process = "End";
            }
        }
    }
}