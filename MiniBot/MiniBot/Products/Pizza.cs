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
        public Pizza(float cost, string name, string description, byte score, List<string> ingredients, short weight, byte size) : base(cost, name, description, score, ingredients, weight)
        {
            Size = size;
        }
    }
}
