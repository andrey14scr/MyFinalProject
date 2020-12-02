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
        public Pizza(string name, float cost, byte score, string[] ingredients, short weight, byte size, string description = "") : base(name, cost, score, ingredients, weight, description)
        {
            Size = size;
        }

        public string GetInfo()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in Ingredients)
            {
                sb.Append(item);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);

            string info = !String.Equals(Description, String.Empty) ? "Description :" + Description + ", " : "";

            return $"{Name}\n" +
                $"Price: {Cost:$0.00}\n" +
                $"Size: {Size}sm\n" +
                $"Weight: {Weight}g\n" +
                info +
                $"Ingredients: {sb.ToString().Trim()}\n" +
                $"Rating: {(float)Score / 2:0.0}*";
        }
    }
}
