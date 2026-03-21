using System.Collections.Generic;
using Shayou.Engine.Core.Domain.Events;
using Shayou.Engine.Core.Runtime.Context;

namespace Shayou.Engine.Core.Domain.Entities
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
            HandAreaName = "";
            EquipmentAreaName = "";
            JudgementAreaName = "";
            Skills = new List<string>();
            TurnPhase = new List<string>();
            DeckName = "";
            Context = null!;
        }

        public Player(int health, int position, string heroName)
        {
            Health = health;
            Position = position;
            HeroName = heroName;
            MaxHealth = health;
            Armor = 0;
            HandAreaName = "";
            EquipmentAreaName = "";
            JudgementAreaName = "";
            Skills = new List<string>();
            TurnPhase = new List<string>();
            DeckName = "";
            Context = null!;
        }

        public void InitializeAreas()
        {
            if (Context == null)
                throw new InvalidOperationException("Player Context is not initialized");

            HandAreaName = $"Player{Position}_Hand";
            EquipmentAreaName = $"Player{Position}_Equipment";
            JudgementAreaName = $"Player{Position}_Judgement";

            Context.Cards.CreateArea(HandAreaName);
            Context.Cards.CreateArea(EquipmentAreaName);
            Context.Cards.CreateArea(JudgementAreaName);
        }

        public List<Card> GetHand()
        {
            return Context.Cards.GetArea(HandAreaName);
        }

        public List<Card> GetEquipment()
        {
            return Context.Cards.GetArea(EquipmentAreaName);
        }

        public List<Card> GetJudgement()
        {
            return Context.Cards.GetArea(JudgementAreaName);
        }

        public void Draw(int count = 1)
        {
            Event drawEvent = new Event("Draw");
            drawEvent.SourcePlayer = this;
            drawEvent.Num = count;
            Context.Services.EventDispatcher.DispatchEvent(drawEvent);
        }
    }
}
