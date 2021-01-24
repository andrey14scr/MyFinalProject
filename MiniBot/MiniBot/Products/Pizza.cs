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
    class Pizza : Food
    {
        public byte Size { get; private set; }
        public Pizza(int id, string name, float cost, byte score, string description, string[] ingredients, short weight, byte size) : base(id, name, cost, score, description, ingredients, weight)
        {
            Size = size;
        }

        protected override string GetInfoWithoutName(string space = "")
        {
            StringBuilder sb = new StringBuilder();
            StringBuilderExtension.Space = space;

            sb.SpaceAppend($"{Sources.Cost}: {Cost:$0.00}" + (Discount > 0 ? $" ({Sources.Discount} {Discount}%)\n" : "\n"));
            if (!String.IsNullOrEmpty(Description))
                sb.SpaceAppend($"{Sources.Description}: {Description}\n");
            sb.SpaceAppend($"{Sources.Composition}: {GetComposition()}\n");
            sb.SpaceAppend($"{Sources.Weight}: {Weight} {Sources.g}\n");
            sb.SpaceAppend($"{Sources.Size}: {Size} cm\n");
            sb.SpaceAppend($"{Sources.Score}: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
