using MiniBot.Activity;
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
        public Pizza(short id, string name, float cost, byte score, string description, string[] ingredients, short weight, byte size) : base(id, name, cost, score, description, ingredients, weight)
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
            Console.WriteLine(this.GetInfoWithoutname(space));
        }

        private string GetInfoWithoutname(string space = "")
        {
            StringBuilder sb = new StringBuilder();
            StringBuilderExtension.Space = space;

            sb.SpaceAppend($"Cost: {Cost:$0.00}\n");
            if (!String.IsNullOrEmpty(Description))
                sb.SpaceAppend($"Description: {Description}\n");
            sb.SpaceAppend($"Composition: {GetComposition()}\n");
            sb.SpaceAppend($"Weight: {Weight} g\n");
            sb.SpaceAppend($"Size: {Size} cm\n");
            sb.SpaceAppend($"Score: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
