using Shayou.Engine.Core.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shayou.Engine.Core.Runtime.Context
{
    public class CardManager
    {
        private readonly Dictionary<string, List<Card>> _areas;
        private readonly List<Card> _allCards;
        private readonly HashSet<Card> _trackedCards;

        public CardManager()
        {
            _areas = new Dictionary<string, List<Card>>();
            _allCards = new List<Card>();
            _trackedCards = new HashSet<Card>();
        }

        public IReadOnlyList<Card> AllCards => _allCards;

        public List<Card> CreateArea(string areaName)
        {
            if (!_areas.ContainsKey(areaName))
            {
                _areas[areaName] = new List<Card>();
            }

            return _areas[areaName];
        }

        public void AddCard(string areaName, Card card)
        {
            if (!_areas.ContainsKey(areaName))
            {
                CreateArea(areaName);
            }

            TrackCard(card);
            card.AreaName = areaName;
            card.MoveTo(areaName);
            _areas[areaName].Add(card);
        }

        public void AddCard(string areaName, IEnumerable<Card> cards)
        {
            if (!_areas.ContainsKey(areaName))
            {
                CreateArea(areaName);
            }

            foreach (var card in cards)
            {
                TrackCard(card);
                card.AreaName = areaName;
                card.MoveTo(areaName);
                _areas[areaName].Add(card);
            }
        }

        public void RemoveCard(string areaName, Card card)
        {
            if (_areas.ContainsKey(areaName))
            {
                _areas[areaName].Remove(card);
            }
        }

        public void RemoveCard(string areaName, IEnumerable<Card> cards)
        {
            if (_areas.ContainsKey(areaName))
            {
                foreach (var card in cards)
                {
                    _areas[areaName].Remove(card);
                }
            }
        }

        public void Shuffle(string areaName)
        {
            if (!_areas.ContainsKey(areaName))
                return;

            Random random = new Random();
            List<Card> area = _areas[areaName];
            int n = area.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = area[k];
                area[k] = area[n];
                area[n] = value;
            }
        }

        public List<Card> GetTopCard(string areaName, int count = 1)
        {
            List<Card> result = new List<Card>();
            if (!_areas.ContainsKey(areaName))
                return result;

            List<Card> area = _areas[areaName];
            int actualCount = Math.Min(count, area.Count);
            for (int i = 0; i < actualCount; i++)
            {
                result.Add(area[i]);
            }

            return result;
        }

        public List<Card> GetBottomCard(string areaName, int count = 1)
        {
            List<Card> result = new List<Card>();
            if (!_areas.ContainsKey(areaName))
                return result;

            List<Card> area = _areas[areaName];
            int actualCount = Math.Min(count, area.Count);
            int startIndex = area.Count - actualCount;
            for (int i = startIndex; i < area.Count; i++)
            {
                result.Add(area[i]);
            }

            return result;
        }

        public List<Card> DrawTopCard(string areaName, int count = 1)
        {
            List<Card> result = new List<Card>();
            if (!_areas.ContainsKey(areaName))
                return result;

            List<Card> area = _areas[areaName];
            int actualCount = Math.Min(count, area.Count);
            for (int i = 0; i < actualCount; i++)
            {
                Card card = area[0];
                result.Add(card);
                area.RemoveAt(0);
                card.AreaName = "";
                card.MoveTo("");
            }

            return result;
        }

        public List<Card> DrawBottomCard(string areaName, int count = 1)
        {
            List<Card> result = new List<Card>();
            if (!_areas.ContainsKey(areaName))
                return result;

            List<Card> area = _areas[areaName];
            int actualCount = Math.Min(count, area.Count);
            for (int i = 0; i < actualCount; i++)
            {
                int lastIndex = area.Count - 1;
                Card card = area[lastIndex];
                result.Add(card);
                area.RemoveAt(lastIndex);
                card.AreaName = "";
                card.MoveTo("");
            }

            return result;
        }

        public List<Card> GetArea(string areaName)
        {
            if (_areas.ContainsKey(areaName))
                return _areas[areaName];

            return new List<Card>();
        }

        public bool HasArea(string areaName)
        {
            return _areas.ContainsKey(areaName);
        }

        public int Count(string areaName)
        {
            if (_areas.ContainsKey(areaName))
                return _areas[areaName].Count;

            return 0;
        }

        public List<string> GetAllAreaNames()
        {
            return new List<string>(_areas.Keys);
        }

        public void LoadFromJson(string filePath, string areaName)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            string json = File.ReadAllText(filePath);
            var config = JsonSerializer.Deserialize<CardConfig>(json);

            if (config != null && config.Cards != null)
            {
                foreach (var cardConfig in config.Cards)
                {
                    Card card = new Card(cardConfig.Suit, cardConfig.Rank, cardConfig.Id, cardConfig.Id);
                    card.Category = cardConfig.Category ?? "";
                    AddCard(areaName, card);
                }

                Console.WriteLine($"Loaded {config.Cards.Count} cards from {filePath} to {areaName}");
            }
        }

        private void TrackCard(Card card)
        {
            if (_trackedCards.Add(card))
            {
                _allCards.Add(card);
            }
        }
    }

    public class CardConfig
    {
        [JsonPropertyName("cards")]
        public List<CardData> Cards { get; set; } = new();
    }

    public class CardData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("suit")]
        public string Suit { get; set; } = "";

        [JsonPropertyName("rank")]
        public int Rank { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = "";
    }
}
