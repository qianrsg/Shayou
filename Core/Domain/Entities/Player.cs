using System.Collections.Generic;
using Shayou.Core.Domain.Models;

namespace Shayou.Core.Domain.Entities
{
    public class Player
    {
        public int Health { get; set; }
        public int Position { get; set; }
        public string HeroName { get; set; }
        public int MaxHealth { get; set; }
        public int Armor { get; set; }
        public string HandAreaName { get; private set; }
        public string EquipmentAreaName { get; private set; }
        public string JudgementAreaName { get; private set; }
        public List<string> Skills { get; private set; }
        public List<string> TurnPhase { get; private set; }
        public string DeckName { get; set; }
        public GameContext Context { get; set; }

        public Player()
        {
            Health = 0;
            Position = 0;
            HeroName = "";
            MaxHealth = 0;
            Armor = 0;
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
            Skills = new List<string>();
            TurnPhase = new List<string>();
        }

        public void InitializeAreas()
        {
            HandAreaName = $"Player{Position}_Hand";
            EquipmentAreaName = $"Player{Position}_Equipment";
            JudgementAreaName = $"Player{Position}_Judgement";

            Context.Zone.CreateArea(HandAreaName);
            Context.Zone.CreateArea(EquipmentAreaName);
            Context.Zone.CreateArea(JudgementAreaName);
        }

        public List<Card> GetHand()
        {
            return Context.Zone.GetArea(HandAreaName);
        }

        public List<Card> GetEquipment()
        {
            return Context.Zone.GetArea(EquipmentAreaName);
        }

        public List<Card> GetJudgement()
        {
            return Context.Zone.GetArea(JudgementAreaName);
        }

        public void Draw(int count = 1)
        {
            Event drawEvent = new Event("Draw");
            drawEvent.SourcePlayer = this;
            drawEvent.Num = count;
            Context.CreateEvent(drawEvent);
        }
    }
}
