using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Interfaces
{
    interface IGetInfo
    {
        string GetInfo(string space = "");

        string GetShortInfo(string space = "");
    }
}
