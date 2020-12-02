using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    abstract class Food : Product
    {
        public string[] Ingredients { get; set; }
        public short Weight { get; set; }

        public Food(string name, float cost, byte score, string[] ingredients, short weight, string description = "") : base(name, cost, score, description)
        {
            if (ingredients.Length == 0)
                throw new Exception();
            Ingredients = ingredients;
            Weight = weight;
        }
    }
}
