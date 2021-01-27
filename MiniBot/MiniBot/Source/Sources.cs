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

            SetComands();
        }

        public static void SetComands()
        {
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
            l = GetLocal("l");

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

        public static string ChoiceLogin; 
        public static string ChoiceRegister;
        public static string ChoicePizza;
        public static string ChoiceSushi;
        public static string ChoiceDrink;
        public static string ChoiceBack;
        public static string ChoiceTake;
        public static string ChoiceSeeBasket;
        public static string ChoiceBuy;
        public static string ChoiceRemove;
        public static string ChoiceReduce;
        public static string ChoiceEnlarge;
        public static string ChoiceExit;

        public static string mailTitle;

        public static string guestName;

        public const string PizzaTable = "PizzaTable";
        public const string SushiTable = "SushiTable";
        public const string DrinkTable = "DrinkTable";

        public const string EN = "en";
        public const string RU = "ru";

        public static string[] ProductTables = new string[] { PizzaTable, SushiTable, DrinkTable };

        public static string Cost;
        public static string Discount;
        public static string Description;
        public static string Volume;
        public static string g;
        public static string l;
        public static string WithGase;
        public static string WithoutGase;
        public static string Alcoholic;
        public static string NotAlcoholic;
        public static string Score;
        public static string Composition;
        public static string Weight;
        public static string Raw;
        public static string Fried;
        public static string Size;
        public static string sm;

        public static string Total;

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
