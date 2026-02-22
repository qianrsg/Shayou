namespace Bang.Core.Domain.Entities
{
    public class Event
    {
        public string Type { get; set; }
        public Card SourceCard { get; set; }
        public Player SourcePlayer { get; set; }
        public Player TargetPlayer { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public string Process { get; private set; }

        public Event()
        {
            Type = "";
            SourceCard = null;
            SourcePlayer = null;
            TargetPlayer = null;
            Data = new Dictionary<string, object>();
            Process = "Entering";
        }

        public Event(string type)
        {
            Type = type;
            SourceCard = null;
            SourcePlayer = null;
            TargetPlayer = null;
            Data = new Dictionary<string, object>();
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
    }
}