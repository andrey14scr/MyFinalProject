using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    abstract class Food : Product
    {
        public string[] Ingredients { get; private set; }
        public short Weight { get; private set; }

        public Food(string name, float cost, byte score, string description, string[] ingredients, short weight) : base(name, cost, score, description)
        {
            if (ingredients.Length == 0)
                throw new Exception();
            Ingredients = ingredients;
            Weight = weight;
        }

        private protected string GetComposition()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in Ingredients)
            {
                sb.Append(item);
                sb.Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }
    }
}
