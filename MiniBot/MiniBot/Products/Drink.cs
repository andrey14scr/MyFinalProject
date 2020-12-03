using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Drink : Product
    {
        public float Volume { get; private set; }
        public bool HasGase { get; private set; }
        public bool IsAlcohol { get; private set; }

        public Drink(string name, float cost, byte score, string description, float volume, bool hasgase, bool isalcohol) : base(name, cost, score, description)
        {
            Volume = volume;
            HasGase = hasgase;
            IsAlcohol = isalcohol;
        }

        private string GetInfo()
        {
            string info = !String.Equals(Description, String.Empty) ? $"Description: {Description}, " : "";
            info += HasGase ? "with gase, " : "without gase, ";
            info += IsAlcohol ? "alcoholic\n" : "no alcohol\n";
            return $"Volume: {Volume:0.0}L\n" +
                $"Price: {Cost:$0.00}\n" +
                info +
                $"Rating: {(float)Score/2:0.0}*";
        }

        public void WriteInfo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Name} ");
            Console.ResetColor();
            Console.WriteLine(GetInfo());
        }
    }
}
