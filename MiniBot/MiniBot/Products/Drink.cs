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

        public Drink(string name, float cost, byte score, string description = "", float volume = 1.0f, bool hasgase = false, bool isalcohol = false) : base(name, cost, score, description)
        {
            Volume = volume;
            HasGase = hasgase;
            IsAlcohol = isalcohol;
        }

        public string GetInfo()
        {
            string info = !String.Equals(Description, String.Empty) ? Description + ", " : "";
            info += HasGase ? "with gase, " : "without gase, ";
            info += IsAlcohol ? "alcoholic" : "no alcohol";
            return $"{Name} {Volume:0.0}L\n" +
                $"Price: {Cost:$0.00}\n" +
                $"Description: {info}\n" +
                $"Rating: {(float)Score/2:0.0}*";
        }
    }
}
