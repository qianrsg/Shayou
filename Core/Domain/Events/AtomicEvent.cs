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
            Timing = "Entering";
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
            Timing = "Entering";
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
