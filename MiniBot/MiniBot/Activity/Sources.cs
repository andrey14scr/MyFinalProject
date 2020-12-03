using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    interface Sources
    {
        enum BotState
        {
            Sleep,
            Write,
            WriteAndWait,
        }
    }
}
