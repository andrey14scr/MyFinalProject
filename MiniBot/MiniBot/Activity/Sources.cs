﻿using System;
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
            Start,
            Write,
            WriteAndWait,
            AccName,
            AccBirthDate,
            AccLogin,
            AccPassword,
            AccountDecision,
            FindAccount,
            ShowMenu,
            AskProduct,
            ShowProduct,
            ProductDecision,
            AskAmount,
            ShowBasket,
            ProductInBusket,
            Confirm,
        }

        public enum ProductType : byte
        {
            Pizza,
            Sushi,
            Drink
        }

        public const string CommandHelp = "-help";
        //public const string CommandYes = "yes";
        //public const string CommandNo = "no";
        public const string CommandExit = "-exit";
        //public const string CommandNew = "new";
        public const string CommandBack = "-back";
        public const string CommandAgree = "-agree";

        public const string ChoiceLogin = "->login";
        public const string ChoiceRegister = "->register";
        public const string ChoicePizza = "->pizza";
        public const string ChoiceSushi = "->sushi";
        public const string ChoiceDrink = "->drink";
        public const string ChoiceBack = "->back";
        public const string ChoiceTake = "->take";
        public const string ChoiceSeeBasket = "->see basket";
        public const string ChoiceBuy = "->buy";
        public const string ChoiceRemove = "->remove";
        public const string ChoiceReduce = "->reduce";
        public const string ChoiceEnlarge = "->enlarge";
        public const string ChoiceExit = "->exit";

        public const string AutoText = "AutoText";

        public const string mailTitle = "Order";

        public const string guestName = "Guest";

        public const string pizzaTable = "PizzaTable";
        public const string sushiTable = "SushiTable";
        public const string drinkTable = "DrinkTable";

        public static bool IsCommand(string command)
        {
            if (Equals(CommandHelp, command) ||
                Equals(CommandBack, command) ||
                Equals(CommandExit, command) ||
                Equals(CommandAgree, command))
            {
                return true;
            }
            return false;
        }
    }
}
