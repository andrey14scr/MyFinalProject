using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    static class Sources
    {
        private static ResourceManager _resourceManager;
        static Sources()
        {
            _resourceManager = new ResourceManager("MiniBot.Resourcers.Localization", Assembly.GetExecutingAssembly());

            ChoiceLogin = "->" + _resourceManager.GetString("login", CultureInfo.CurrentCulture);
            ChoiceRegister = "->" + _resourceManager.GetString("register", CultureInfo.CurrentCulture);
            ChoicePizza = "->" + _resourceManager.GetString("pizza", CultureInfo.CurrentCulture);
            ChoiceSushi = "->" + _resourceManager.GetString("sushi", CultureInfo.CurrentCulture);
            ChoiceDrink = "->" + _resourceManager.GetString("drink", CultureInfo.CurrentCulture);
            ChoiceBack = "->" + _resourceManager.GetString("back", CultureInfo.CurrentCulture);
            ChoiceTake = "->" + _resourceManager.GetString("take", CultureInfo.CurrentCulture);
            ChoiceSeeBasket = "->" + _resourceManager.GetString("see basket", CultureInfo.CurrentCulture);
            ChoiceBuy = "->" + _resourceManager.GetString("buy", CultureInfo.CurrentCulture);
            ChoiceRemove = "->" + _resourceManager.GetString("remove", CultureInfo.CurrentCulture);
            ChoiceReduce = "->" + _resourceManager.GetString("reduce", CultureInfo.CurrentCulture);
            ChoiceEnlarge = "->" + _resourceManager.GetString("enlarge", CultureInfo.CurrentCulture);
            ChoiceExit = "->" + _resourceManager.GetString("exit", CultureInfo.CurrentCulture);

            mailTitle = _resourceManager.GetString("Order", CultureInfo.CurrentCulture);

            guestName = _resourceManager.GetString("Guest", CultureInfo.CurrentCulture);

            Cost = _resourceManager.GetString("Cost", CultureInfo.CurrentCulture);
            Discount = _resourceManager.GetString("Discount", CultureInfo.CurrentCulture);
            Description = _resourceManager.GetString("Description", CultureInfo.CurrentCulture); 
            Volume = _resourceManager.GetString("Volume", CultureInfo.CurrentCulture);
            g = _resourceManager.GetString("g", CultureInfo.CurrentCulture);
            WithGase = _resourceManager.GetString("With gase", CultureInfo.CurrentCulture);
            WithoutGase = _resourceManager.GetString("Without gase", CultureInfo.CurrentCulture);
            Alcoholic = _resourceManager.GetString("Alcoholic", CultureInfo.CurrentCulture);
            NotAlcoholic = _resourceManager.GetString("Not alcoholic", CultureInfo.CurrentCulture);
            Score = _resourceManager.GetString("Score", CultureInfo.CurrentCulture);
            Composition = _resourceManager.GetString("Composition", CultureInfo.CurrentCulture);
            Weight = _resourceManager.GetString("Weight", CultureInfo.CurrentCulture);
            Raw = _resourceManager.GetString("Raw", CultureInfo.CurrentCulture);
            Fried = _resourceManager.GetString("Fried", CultureInfo.CurrentCulture);
            Size = _resourceManager.GetString("Size", CultureInfo.CurrentCulture);
            sm = _resourceManager.GetString("sm", CultureInfo.CurrentCulture);
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
