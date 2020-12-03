using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Interfaces
{
    interface IBot
    {
        void SendMessage(string msg);
        void GetAnswer(string answer);
        void DoAction();
    }
}
