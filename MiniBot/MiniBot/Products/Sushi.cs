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
        public Sushi(float cost, string name, string description, byte score, List<string> ingredients, short weight, byte size, bool israw) : base(cost, name, description, score, ingredients, weight)
        {
            IsRaw = israw;
        }
    }
}
