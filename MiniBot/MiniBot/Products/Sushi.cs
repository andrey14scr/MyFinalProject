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
        public Sushi(string name, float cost, byte score, string[] ingredients, short weight, byte size, bool israw, string description = "") : base(name, cost, score, ingredients, weight, description)
        {
            IsRaw = israw;
        }
    }
}
