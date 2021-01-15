using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Interfaces
{
    interface IShowInfo
    {
        void ShowShortInfo(string space = "");

        void ShowInfo(string space = "", bool isSpacedName = false);
    }
}
