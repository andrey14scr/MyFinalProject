using MiniBot.Activity;
using System;
using System.Text;

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

            sb.SpaceAppend($"{Sources.Cost}: {Cost:$0.00}" + (Discount > 0 ? $" ({Sources.Discount} {Discount}%)\n" : "\n"));
            if (!String.IsNullOrEmpty(Description))
                sb.SpaceAppend($"{Sources.Description}: {Description}\n");
            sb.SpaceAppend($"{Sources.Composition}: {GetComposition()}\n");
            sb.SpaceAppend($"{Sources.Weight}: {Weight} {Sources.g}\n");
            sb.SpaceAppend(IsRaw ? $"{Sources.Raw}\n" : $"{Sources.Fried}\n");
            sb.SpaceAppend($"{Sources.Score}: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
