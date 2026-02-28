namespace Bang.Core.Domain.Entities
{
    public class AtomicEvent : BaseEvent
    {
        public AtomicEvent()
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

        public AtomicEvent(string name)
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
                Callback?.Invoke(this);
                Process = "Finished";
            }
            else
            {
                Process = "End";
            }
        }
    }
}
