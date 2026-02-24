namespace Bang.Core.Domain.Entities
{
    public class Event
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
        public string Process { get; private set; }

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

        public void AdvanceProcess()
        {
            if (Process == "Entering")
            {
                Process = "Processing";
            }
            else if (Process == "Processing")
            {
                Process = "Processed";
            }
            else if (Process == "Processed")
            {
                Process = "Exiting";
            }
            else if (Process == "Exiting")
            {
                Process = "Finished";
            }
        }

        public bool IsFinished()
        {
            return Process == "Finished";
        }

        public void Stop()
        {
            Process = "Finished";
        }
    }
}