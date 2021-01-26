using MiniBot.Products;
using System;
using System.Collections.Generic;
namespace MiniBot.Interfaces
{
    interface IDBProduct
    {
        void AddToDB(Product item);

        IEnumerable<Product> GetFromDB(Func<Product, bool> predicate);

        void RemoveFromDB(int id);
    }
}
