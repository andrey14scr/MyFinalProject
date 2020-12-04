using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Interfaces
{
    interface IDBWork
    {
        void AddToDB(object product);
        object GetFromDB();
        void RemoveFromDB(object product);
    }
}
