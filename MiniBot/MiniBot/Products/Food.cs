using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Food : Product
    {
        public List<string> Ingredients { get; set; }
        public short Weight { get; set; }
    }
}
