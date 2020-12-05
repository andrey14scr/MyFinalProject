using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Pizza : Food
    {
        public byte Size { get; private set; }
        public Pizza(string name, float cost, byte score, string description, string[] ingredients, short weight, byte size) : base(name, cost, score, description, ingredients, weight)
        {
            Size = size;
        }

        private string GetInfo(string space)
        {
            string info = !String.Equals(Description, String.Empty) ? $"{space}Description: {Description}\n" : "";

            return $"{space}Price: {Cost:$0.00}\n" +
                $"{space}Size: {Size}sm\n" +
                $"{space}Weight: {Weight}g\n" +
                info +
                $"{space}Ingredients: {this.GetComposition()}\n" +
                $"{space}Rating: {(float)Score / 2:0.0}*";
        }

        public void WriteInfo(string space)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{space}{Name} ");
            Console.ResetColor();
            Console.WriteLine(GetInfo(space));
        }
    }
}
