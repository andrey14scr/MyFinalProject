using MiniBot.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Interfaces
{
    interface IOnlineShop
    {
        void Buy(Product product, byte amount = 1);
        void Estimate(Product product, byte score);
    }
}
