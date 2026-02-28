using System.Collections.Generic;
using Bang.Core.Domain.Models;

namespace Bang.Core.Domain.Entities
{
    public class Player
    {
        public int Health { get; set; }
        public int Position { get; set; }
        public string HeroName { get; set; }
        public int MaxHealth { get; set; }
        public int Armor { get; set; }
        public List<Card> Hand { get; private set; }
        public Dictionary<string, Card> Equipment { get; private set; }
        public List<Card> Judgement { get; private set; }
        public List<string> Skills { get; private set; }
        public List<string> TurnPhase { get; private set; }
        public string Pile { get; set; }
        public IContext Context { get; set; }

        public Player()
        {
            Health = 0;
            Position = 0;
            HeroName = "";
            MaxHealth = 0;
            Armor = 0;
            Hand = new List<Card>();
            Equipment = new Dictionary<string, Card>();
            Judgement = new List<Card>();
            Skills = new List<string>();
            TurnPhase = new List<string>();
        }

        public Player(int health, int position, string heroName)
        {
            Health = health;
            Position = position;
            HeroName = heroName;
            MaxHealth = health;
            Armor = 0;
            Hand = new List<Card>();
            Equipment = new Dictionary<string, Card>();
            Judgement = new List<Card>();
            Skills = new List<string>();
            TurnPhase = new List<string>();
        }

        public void Draw(int count = 1)
        {
            Event drawEvent = new Event("Draw");
            drawEvent.Num = count;
            Context.CreateEvent(drawEvent);
        }
    }
}