using MiniBot.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Interfaces
{
    interface IDBWork
    {
        void AddToDB(object product);
        void GetFromDB(Action<Product, short> action, ProductType producttype);
        void RemoveFromDB(object product);
    }
}
