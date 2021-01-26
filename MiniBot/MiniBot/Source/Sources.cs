using LogInfo;
using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace MiniBot.Activity
{
    static class Sources
    {
        public static ResourceManager ResourceManager;
        static Sources()
        {
            try
            {
                ResourceManager = new ResourceManager("MiniBot.Resources.Localization", Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                if (!Logger.IsInited)
                    Logger.Init();
                Logger.Error(ex.Message);
                Environment.Exit(0);
            }
            ChoiceLogin = "->" + GetLocal("login");
            ChoiceRegister = "->" + GetLocal("register");
            ChoicePizza = "->" + GetLocal("pizza");
            ChoiceSushi = "->" + GetLocal("sushi");
            ChoiceDrink = "->" + GetLocal("drink");
            ChoiceBack = "->" + GetLocal("back");
            ChoiceTake = "->" + GetLocal("take");
            ChoiceSeeBasket = "->" + GetLocal("see basket");
            ChoiceBuy = "->" + GetLocal("buy");
            ChoiceRemove = "->" + GetLocal("remove");
            ChoiceReduce = "->" + GetLocal("reduce");
            ChoiceEnlarge = "->" + GetLocal("enlarge");
            ChoiceExit = "->" + GetLocal("exit");

            mailTitle = GetLocal("Order");

            guestName = GetLocal("Guest");

            Cost = GetLocal("Cost");
            Discount = GetLocal("Discount");
            Description = GetLocal("Description"); 
            Volume = GetLocal("Volume");
            g = GetLocal("g");
            WithGase = GetLocal("With gase");
            WithoutGase = GetLocal("Without gase");
            Alcoholic = GetLocal("Alcoholic");
            NotAlcoholic = GetLocal("Not alcoholic");
            Score = GetLocal("Score");
            Composition = GetLocal("Composition");
            Weight = GetLocal("Weight");
            Raw = GetLocal("Raw");
            Fried = GetLocal("Fried");
            Size = GetLocal("Size");
            sm = GetLocal("sm");

            Total = GetLocal("Total");
        }

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
            AskAdress,
        }

        public enum ProductType : byte
        {
            Pizza,
            Sushi,
            Drink
        }

        public const string CommandHelp = "-help";
        public const string CommandExit = "-exit";
        public const string CommandBack = "-back";
        public const string CommandAgree = "-agree";

        public static readonly string ChoiceLogin; 
        public static readonly string ChoiceRegister;
        public static readonly string ChoicePizza;
        public static readonly string ChoiceSushi;
        public static readonly string ChoiceDrink;
        public static readonly string ChoiceBack;
        public static readonly string ChoiceTake;
        public static readonly string ChoiceSeeBasket;
        public static readonly string ChoiceBuy;
        public static readonly string ChoiceRemove;
        public static readonly string ChoiceReduce;
        public static readonly string ChoiceEnlarge;
        public static readonly string ChoiceExit;

        public static readonly string mailTitle;

        public static readonly string guestName;

        public const string PizzaTable = "PizzaTable";
        public const string SushiTable = "SushiTable";
        public const string DrinkTable = "DrinkTable";

        public const string EN = "en";
        public const string RU = "ru";

        public static string[] ProductTables = new string[] { PizzaTable, SushiTable, DrinkTable };

        public static readonly string Cost;
        public static readonly string Discount;
        public static readonly string Description;
        public static readonly string Volume;
        public static readonly string g;
        public static readonly string WithGase;
        public static readonly string WithoutGase;
        public static readonly string Alcoholic;
        public static readonly string NotAlcoholic;
        public static readonly string Score;
        public static readonly string Composition;
        public static readonly string Weight;
        public static readonly string Raw;
        public static readonly string Fried;
        public static readonly string Size;
        public static readonly string sm;

        public static readonly string Total;

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

        public static string GetLocal(string name)
        {
            string answer = "";

            try
            {
                answer = ResourceManager.GetString(name, CultureInfo.CurrentCulture);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return answer;
        }
    }
}
