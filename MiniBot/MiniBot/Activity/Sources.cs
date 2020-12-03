using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    static class Sources
    {
        public enum BotState
        {
            Sleep,
            Write,
            WriteAndWait,
            CreateAccount,
            AccName,
            AccLogin,
            AccPassword,
            AccountDecision
        }

        public static string CommandHelp = "help";
        public static string CommandYes = "yes";
        public static string CommandNo = "no";
        public static string CommandExit = "exit";
        public static string CommandExisting = "existing";
        public static string CommandNew = "new";
    }
}
