using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    abstract class Food : Product
    {
        public List<string> Ingredients { get; set; }
        public short Weight { get; set; }

        public Food(float cost, string name, string description, byte score, List<string> ingredients, short weight) : base(cost, name, description, score)
        {
            Ingredients = ingredients;
            Weight = weight;
        }
    }
}
