namespace Bang.Core.Domain.Entities
{
    public class Card
    {
        public string Suit { get; set; }
        public int Rank { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string AreaName { get; set; }
        private string currentArea;

        public Card()
        {
            Suit = "";
            Rank = 0;
            Id = "";
            Name = "";
            Category = "";
            AreaName = "";
            currentArea = "";
        }

        public Card(string suit, int rank, string id, string name)
        {
            Suit = suit;
            Rank = rank;
            Id = id;
            Name = name;
            Category = "";
            AreaName = "";
            currentArea = "";
        }

        public string GetCurrentArea()
        {
            return currentArea;
        }

        public void MoveTo(string areaName)
        {
            currentArea = areaName;
        }

        public void PrintInfo()
        {
            Console.WriteLine("   Card Properties:");
            Console.WriteLine($"     Suit: {Suit}");
            Console.WriteLine($"     Rank: {Rank}");
            Console.WriteLine($"     Id: {Id}");
            Console.WriteLine($"     Name: {Name}");
            Console.WriteLine($"     Current Area: {currentArea}");
        }
    }
}