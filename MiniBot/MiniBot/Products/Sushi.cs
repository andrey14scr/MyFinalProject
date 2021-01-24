using MiniBot.Activity;
using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Sushi : Food
    {
        public bool IsRaw { get; private set; }
        public Sushi(int id, string name, float cost, byte score, string description, string[] ingredients, short weight, bool israw) : base(id, name, cost, score, description, ingredients, weight)
        {
            IsRaw = israw;
        }

        protected override string GetInfoWithoutName(string space = "")
        {
            StringBuilder sb = new StringBuilder();
            StringBuilderExtension.Space = space;

            sb.SpaceAppend($"Cost: {Cost:$0.00}" + (Discount > 0 ? $" (Discount {Discount}%)\n" : "\n"));
            if (!String.IsNullOrEmpty(Description))
                sb.SpaceAppend($"Description: {Description}\n");
            sb.SpaceAppend($"Composition: {GetComposition()}\n");
            sb.SpaceAppend($"Weight: {Weight} g\n");
            sb.SpaceAppend(IsRaw ? "Raw\n" : "Fried\n");
            sb.SpaceAppend($"Score: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
