using System.Collections.Generic;

namespace Bang.Core.Domain.Entities
{
    public class Player
    {
        public int Health { get; set; }
        public int Position { get; set; }
        public string HeroName { get; set; }
        public int MaxHealth { get; set; }
        public List<Card> Hand { get; private set; }
        public Dictionary<string, Card> Equipment { get; private set; }
        public List<Card> Judgement { get; private set; }
        public List<string> Skills { get; private set; }
        public List<string> TurnPhase { get; private set; }

        public Player()
        {
            Health = 0;
            Position = 0;
            HeroName = "";
            MaxHealth = 0;
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
            Hand = new List<Card>();
            Equipment = new Dictionary<string, Card>();
            Judgement = new List<Card>();
            Skills = new List<string>();
            TurnPhase = new List<string>();
        }

        public void AddToHand(Card card)
        {
            Hand.Add(card);
            card.MoveTo("Hand");
        }

        public void AddCardsToHand(List<Card> cards)
        {
            Hand.AddRange(cards);
        }

        public void RemoveFromHand(Card card)
        {
            Hand.Remove(card);
        }

        public void RemoveCardsFromHand(List<Card> cards)
        {
            foreach (var card in cards)
            {
                Hand.Remove(card);
            }
        }

        public List<Card> GetHandCards()
        {
            return new List<Card>(Hand);
        }

        public int GetHandCardCount()
        {
            return Hand.Count;
        }

        public void AddToEquipment(string slot, Card card)
        {
            Equipment[slot] = card;
            card.MoveTo("Equipment");
        }

        public void RemoveFromEquipment(string slot)
        {
            Equipment.Remove(slot);
        }

        public Card GetEquipmentCard(string slot)
        {
            return Equipment.ContainsKey(slot) ? Equipment[slot] : null;
        }

        public Dictionary<string, Card> GetEquipment()
        {
            return new Dictionary<string, Card>(Equipment);
        }

        public void AddToJudgement(Card card)
        {
            Judgement.Add(card);
            card.MoveTo("Judgement");
        }

        public void RemoveFromJudgement(Card card)
        {
            Judgement.Remove(card);
        }

        public List<Card> GetJudgementCards()
        {
            return new List<Card>(Judgement);
        }

        public int GetJudgementCardCount()
        {
            return Judgement.Count;
        }

        public void AddSkill(string skill)
        {
            Skills.Add(skill);
        }

        public void RemoveSkill(string skill)
        {
            Skills.Remove(skill);
        }

        public bool HasSkill(string skill)
        {
            return Skills.Contains(skill);
        }

        public List<string> GetAllSkills()
        {
            return new List<string>(Skills);
        }

        public void PrintInfo()
        {
            Console.WriteLine($"   Player: {HeroName}");
            Console.WriteLine($"   Health: {Health}/{MaxHealth}");
            Console.WriteLine($"   Position: {Position}");
            Console.WriteLine($"   Skills: {string.Join(", ", Skills)}");
            Console.WriteLine($"   Hand: {Hand.Count} cards");
            Console.WriteLine($"   Equipment: {Equipment.Count} cards");
            Console.WriteLine($"   Judgement: {Judgement.Count} cards");
        }
    }
}