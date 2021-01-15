using MiniBot.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Interfaces
{
    interface IDBProduct
    {
        void AddToDB(Product item);

        IEnumerable<Product> GetFromDB(Func<Product, bool> predicate);

        void RemoveFromDB(Func<Product, bool> predicate);
    }
}
