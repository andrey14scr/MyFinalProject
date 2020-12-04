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
            FindAccount,
            ShowMenu,
            AskProduct,
        }

        public const string CommandHelp = "-help";
        //public const string CommandYes = "yes";
        //public const string CommandNo = "no";
        public const string CommandExit = "-exit";
        //public const string CommandNew = "new";
        public const string CommandBack = "-back";

        public const string ChoiceExisting = "->login";
        public const string ChoiceCreate = "->register";
        public const string ChoicePizza = "->pizza";
        public const string ChoiceSushi = "->sushi";
        public const string ChoiceDrink = "->drink";

        public static bool IsCommand(string command)
        {
            if (Equals(CommandHelp, command) ||
                Equals(CommandBack, command) ||
                Equals(CommandExit, command))
            {
                return true;
            }
            return false;
        }
    }
}
