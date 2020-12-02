using MiniBot.Interfaces;
using MiniBot.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot
{
    class OnlineShop : IOnlineShop
    {
        public List<Product> ProductRange { get; private set; } = new List<Product>();

        public void Buy(Product product, byte amount = 1)
        {
            throw new NotImplementedException();
        }

        public void Estimate(Product product, byte score)
        {
            throw new NotImplementedException();
        }
    }
}
