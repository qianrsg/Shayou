using System.Collections.Generic;

namespace Bang.Core.Domain.Entities
{
    public class Card
    {
        public Dictionary<string, int> IntProperties { get; private set; }
        public Dictionary<string, string> StringProperties { get; private set; }
        private string currentArea;

        public Card()
        {
            IntProperties = new Dictionary<string, int>();
            StringProperties = new Dictionary<string, string>();
            currentArea = "";
        }

        public Card(Dictionary<string, int> intProps, Dictionary<string, string> stringProps)
        {
            IntProperties = new Dictionary<string, int>(intProps);
            StringProperties = new Dictionary<string, string>(stringProps);
            currentArea = "";
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
            Console.WriteLine("     String Properties:");
            foreach (var prop in StringProperties)
            {
                Console.WriteLine($"       {prop.Key}: {prop.Value}");
            }
            Console.WriteLine("     Int Properties:");
            foreach (var prop in IntProperties)
            {
                Console.WriteLine($"       {prop.Key}: {prop.Value}");
            }
            Console.WriteLine($"     Current Area: {currentArea}");
        }
    }
}