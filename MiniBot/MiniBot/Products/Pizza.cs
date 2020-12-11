using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Pizza : Food, IShowInfo, IGetInfo
    {
        public byte Size { get; private set; }
        public Pizza(string name, float cost, byte score, string description, string[] ingredients, short weight, byte size) : base(name, cost, score, description, ingredients, weight)
        {
            Size = size;
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
            sb.Append($"Size: {Size} cm\n");
            sb.Append($"Score: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
