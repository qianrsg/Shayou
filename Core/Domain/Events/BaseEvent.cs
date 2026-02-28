namespace Bang.Core.Domain.Entities
{
    public abstract class BaseEvent
    {
        public string Name { get; set; }
        public Card SourceCard { get; set; }
        public List<Card> Cards { get; set; }
        public Player SourcePlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public List<Card> SourceContainer { get; set; }
        public List<Card> TargetContainer { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public int Num { get; set; }
        public string Timing { get; protected set; }
        public Action<BaseEvent> Callback { get; set; }

        protected BaseEvent()
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

        protected BaseEvent(string name)
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

        public abstract void AdvanceProcess();

        public bool IsEnd()
        {
            return Timing == "End";
        }

        public void Cancel()
        {
            Timing = "Cancelled";
        }
    }
}
