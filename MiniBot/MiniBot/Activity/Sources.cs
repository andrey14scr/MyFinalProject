using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    static class Sources
    {
        public enum BotState : byte
        {
            Sleep,
            Write,
            WriteAndWait,
            AccName,
            AccLogin,
            AccPassword,
            AccountDecision,
            FindAccount
        }

        public const string CommandHelp = "help";
        //public const string CommandYes = "yes";
        //public const string CommandNo = "no";
        public const string CommandExit = "exit";
        public const string ChoiceExisting = "->existing";
        //public const string CommandNew = "new";
        public const string ChoiceCreate = "->create";
        public const string CommandBack = "back";
    }
}
