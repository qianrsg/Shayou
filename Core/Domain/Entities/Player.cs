using System.Collections.Generic;

namespace Bang.Core.Domain.Entities
{
    public class Player
    {
        public Dictionary<string, int> IntProperties { get; private set; }
        public Dictionary<string, string> StringProperties { get; private set; }

        public Player()
        {
            IntProperties = new Dictionary<string, int>();
            StringProperties = new Dictionary<string, string>();
        }

        public Player(int health, int position, string heroName)
        {
            IntProperties = new Dictionary<string, int>();
            StringProperties = new Dictionary<string, string>();
            IntProperties["Health"] = health;
            IntProperties["Position"] = position;
            StringProperties["HeroName"] = heroName;
            IntProperties["MaxHealth"] = health;
            Areas = new Dictionary<string, List<Card>>();
            Skills = new List<string>();
        }

        public void SetProperty(string key, int value)
        {
            IntProperties[key] = value;
        }

        public void SetProperty(string key, string value)
        {
            StringProperties[key] = value;
        }

        public int GetIntProperty(string key)
        {
            return IntProperties.ContainsKey(key) ? IntProperties[key] : 0;
        }

        public string GetStringProperty(string key)
        {
            return StringProperties.ContainsKey(key) ? StringProperties[key] : "";
        }

        public Dictionary<string, List<Card>> Areas { get; private set; }
        public List<string> Skills { get; private set; }

        public void AddCardToArea(string areaName, Card card)
        {
            if (!Areas.ContainsKey(areaName))
            {
                Areas[areaName] = new List<Card>();
            }
            Areas[areaName].Add(card);
            card.MoveTo(areaName);
        }

        public void AddCardsToArea(string areaName, List<Card> cards)
        {
            if (!Areas.ContainsKey(areaName))
            {
                Areas[areaName] = new List<Card>();
            }
            Areas[areaName].AddRange(cards);
        }

        public void RemoveCardFromArea(string areaName, Card card)
        {
            if (Areas.ContainsKey(areaName))
            {
                Areas[areaName].Remove(card);
            }
        }

        public void RemoveCardsFromArea(string areaName, List<Card> cards)
        {
            if (Areas.ContainsKey(areaName))
            {
                foreach (var card in cards)
                {
                    Areas[areaName].Remove(card);
                }
            }
        }

        public List<Card> GetAreaCards(string areaName)
        {
            return Areas.ContainsKey(areaName) ? new List<Card>(Areas[areaName]) : new List<Card>();
        }

        public int GetAreaCardCount(string areaName)
        {
            return Areas.ContainsKey(areaName) ? Areas[areaName].Count : 0;
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
            Console.WriteLine($"   Player: {GetStringProperty("HeroName")}");
            Console.WriteLine($"   Health: {GetIntProperty("Health")}/{GetIntProperty("MaxHealth")}");
            Console.WriteLine($"   Position: {GetIntProperty("Position")}");
            Console.WriteLine($"   Skills: {string.Join(", ", Skills)}");
            Console.WriteLine($"   Areas:");
            foreach (var area in Areas)
            {
                Console.WriteLine($"     {area.Key}: {area.Value.Count} cards");
            }
        }
    }
}