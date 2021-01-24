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
        public static ResourceManager ResourceManager;
        static Sources()
        {
            ResourceManager = new ResourceManager("MiniBot.Resources.Localization", Assembly.GetExecutingAssembly());

            ChoiceLogin = "->" + ResourceManager.GetString("login", CultureInfo.CurrentCulture);
            ChoiceRegister = "->" + ResourceManager.GetString("register", CultureInfo.CurrentCulture);
            ChoicePizza = "->" + ResourceManager.GetString("pizza", CultureInfo.CurrentCulture);
            ChoiceSushi = "->" + ResourceManager.GetString("sushi", CultureInfo.CurrentCulture);
            ChoiceDrink = "->" + ResourceManager.GetString("drink", CultureInfo.CurrentCulture);
            ChoiceBack = "->" + ResourceManager.GetString("back", CultureInfo.CurrentCulture);
            ChoiceTake = "->" + ResourceManager.GetString("take", CultureInfo.CurrentCulture);
            ChoiceSeeBasket = "->" + ResourceManager.GetString("see basket", CultureInfo.CurrentCulture);
            ChoiceBuy = "->" + ResourceManager.GetString("buy", CultureInfo.CurrentCulture);
            ChoiceRemove = "->" + ResourceManager.GetString("remove", CultureInfo.CurrentCulture);
            ChoiceReduce = "->" + ResourceManager.GetString("reduce", CultureInfo.CurrentCulture);
            ChoiceEnlarge = "->" + ResourceManager.GetString("enlarge", CultureInfo.CurrentCulture);
            ChoiceExit = "->" + ResourceManager.GetString("exit", CultureInfo.CurrentCulture);

            mailTitle = ResourceManager.GetString("Order", CultureInfo.CurrentCulture);

            guestName = ResourceManager.GetString("Guest", CultureInfo.CurrentCulture);

            Cost = ResourceManager.GetString("Cost", CultureInfo.CurrentCulture);
            Discount = ResourceManager.GetString("Discount", CultureInfo.CurrentCulture);
            Description = ResourceManager.GetString("Description", CultureInfo.CurrentCulture); 
            Volume = ResourceManager.GetString("Volume", CultureInfo.CurrentCulture);
            g = ResourceManager.GetString("g", CultureInfo.CurrentCulture);
            WithGase = ResourceManager.GetString("With gase", CultureInfo.CurrentCulture);
            WithoutGase = ResourceManager.GetString("Without gase", CultureInfo.CurrentCulture);
            Alcoholic = ResourceManager.GetString("Alcoholic", CultureInfo.CurrentCulture);
            NotAlcoholic = ResourceManager.GetString("Not alcoholic", CultureInfo.CurrentCulture);
            Score = ResourceManager.GetString("Score", CultureInfo.CurrentCulture);
            Composition = ResourceManager.GetString("Composition", CultureInfo.CurrentCulture);
            Weight = ResourceManager.GetString("Weight", CultureInfo.CurrentCulture);
            Raw = ResourceManager.GetString("Raw", CultureInfo.CurrentCulture);
            Fried = ResourceManager.GetString("Fried", CultureInfo.CurrentCulture);
            Size = ResourceManager.GetString("Size", CultureInfo.CurrentCulture);
            sm = ResourceManager.GetString("sm", CultureInfo.CurrentCulture);

            Total = ResourceManager.GetString("Total", CultureInfo.CurrentCulture);
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
    }
}
