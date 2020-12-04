using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Interfaces
{
    interface IBot
    {
        void SendMessage(string msg, BotState nextbotstate);
        void GetAnswer();
        void DoAction(string command);
    }
}
