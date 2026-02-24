using Bang.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bang.Core.Domain.Entities
{
    public class Pile
    {
        private Dictionary<string, List<Card>> piles;

        public Pile()
        {
            piles = new Dictionary<string, List<Card>>();
        }

        public void CreatePile(string pileName)
        {
            if (!piles.ContainsKey(pileName))
            {
                piles[pileName] = new List<Card>();
            }
        }

        public void CreatePilePair(string pileName)
        {
            CreatePile($"{pileName}_Deck");
            CreatePile($"{pileName}_Discard");
        }

        public void AddCard(string pileName, Card card)
        {
            if (!piles.ContainsKey(pileName))
            {
                piles[pileName] = new List<Card>();
            }
            card.PileName = pileName;
            piles[pileName].Add(card);
        }

        public void AddCards(string pileName, List<Card> newCards)
        {
            if (!piles.ContainsKey(pileName))
            {
                piles[pileName] = new List<Card>();
            }
            foreach (var card in newCards)
            {
                card.PileName = pileName;
            }
            piles[pileName].AddRange(newCards);
        }

        public void RemoveCard(string pileName, Card card)
        {
            if (piles.ContainsKey(pileName))
            {
                piles[pileName].Remove(card);
            }
        }

        public void RemoveCards(string pileName, List<Card> cardsToRemove)
        {
            if (piles.ContainsKey(pileName))
            {
                foreach (var card in cardsToRemove)
                {
                    piles[pileName].Remove(card);
                }
            }
        }

        public void Shuffle(string pileName)
        {
            if (!piles.ContainsKey(pileName))
                return;

            Random random = new Random();
            List<Card> pile = piles[pileName];
            int n = pile.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = pile[k];
                pile[k] = pile[n];
                pile[n] = value;
            }
        }

        public Card DrawCard(string pileName)
        {
            if (!piles.ContainsKey(pileName) || piles[pileName].Count == 0)
                return null;
            List<Card> pile = piles[pileName];
            Card card = pile[0];
            pile.RemoveAt(0);
            return card;
        }

        public List<Card> DrawCards(string pileName, int count)
        {
            List<Card> drawnCards = new List<Card>();
            for (int i = 0; i < count; i++)
            {
                Card card = DrawCard(pileName);
                if (card != null)
                {
                    drawnCards.Add(card);
                }
            }
            return drawnCards;
        }

        public Card GetCard(string pileName, Func<Card, bool> predicate)
        {
            if (!piles.ContainsKey(pileName))
                return null;

            foreach (var card in piles[pileName])
            {
                if (predicate(card))
                {
                    return card;
                }
            }
            return null;
        }

        public int Count(string pileName)
        {
            return piles.ContainsKey(pileName) ? piles[pileName].Count : 0;
        }

        public List<Card> GetPile(string pileName)
        {
            return piles.ContainsKey(pileName) ? piles[pileName] : new List<Card>();
        }

        public List<string> GetAllPileNames()
        {
            return new List<string>(piles.Keys);
        }

        public int TotalCount()
        {
            int total = 0;
            foreach (var pile in piles.Values)
            {
                total += pile.Count;
            }
            return total;
        }

        public void LoadFromJson(string filePath, string pileName)
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
                    AddCard(pileName, card);
                }
                Console.WriteLine($"Loaded {config.Cards.Count} cards from {filePath} to {pileName}");
            }
        }
    }

    public class CardConfig
    {
        [JsonPropertyName("cards")]
        public List<CardData> Cards { get; set; }
    }

    public class CardData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("suit")]
        public string Suit { get; set; }
        [JsonPropertyName("rank")]
        public int Rank { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}