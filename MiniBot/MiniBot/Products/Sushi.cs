using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Sushi : Food, IShowInfo, IGetInfo
    {
        public bool IsRaw { get; private set; }
        public Sushi(string name, float cost, byte score, string description, string[] ingredients, short weight, bool israw) : base(name, cost, score, description, ingredients, weight)
        {
            IsRaw = israw;
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
            sb.Append($"Composition: {GetComposition()}\n");
            sb.Append($"Weight: {Weight} g\n");
            sb.Append(IsRaw ? "Raw\n" : "Fried\n");
            sb.Append($"Score: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
