using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Drink : Product, IShowInfo, IGetInfo
    {
        public float Volume { get; private set; }
        public bool HasGase { get; private set; }
        public bool IsAlcohol { get; private set; }

        public Drink(string name, float cost, byte score, string description, float volume, bool hasgase, bool isalcohol) : base(name, cost, score, description)
        {
            Volume = volume;
            HasGase = hasgase;
            IsAlcohol = isalcohol;
        }

        public string GetInfo(string space = "")
        {
            return Name + "\n" + GetInfoWithoutname();
        }

        public string GetShortInfo(string space = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(space);
            sb.Append(Name);
            sb.Append($" {Cost:$0.00}");

            return sb.ToString();
        }

        public override void ShowShortInfo(string space = "")
        {
            Console.WriteLine($"{space}{Name} {Cost:$0.00}");
        }

        public override void ShowInfo(string space = "")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{space}{Name}");
            Console.ResetColor();
            Console.WriteLine(this.GetInfoWithoutname());
        }

        private string GetInfoWithoutname()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Cost: {Cost:$0.00}\n");
            if (!String.IsNullOrEmpty(Description))
                sb.Append($"Description: {Description}\n");
            sb.Append($"Volume: {Volume} g\n");
            sb.Append(HasGase ? "With gase\n" : "Without gase\n");
            sb.Append(IsAlcohol ? "Alcoholic\n" : "Not alcoholic\n");
            sb.Append($"Score: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
