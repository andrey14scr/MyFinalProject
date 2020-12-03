using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Sushi : Food
    {
        public bool IsRaw { get; private set; }
        public Sushi(string name, float cost, byte score, string description, string[] ingredients, short weight, bool israw) : base(name, cost, score, description, ingredients, weight)
        {
            IsRaw = israw;
        }

        private string GetInfo()
        {
            string info = !String.Equals(Description, String.Empty) ? $"Description: {Description}, " : "";
            info += IsRaw ? "raw\n" : "fried\n";

            return $"Price: {Cost:$0.00}\n" +
                $"Weight: {Weight}g\n" +
                info +
                $"Ingredients: {this.GetComposition()}\n" +
                $"Rating: {(float)Score / 2:0.0}*";
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
