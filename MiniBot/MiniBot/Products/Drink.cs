using MiniBot.Activity;
using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Drink : Product
    {
        public float Volume { get; private set; }
        public bool HasGase { get; private set; }
        public bool IsAlcohol { get; private set; }

        public Drink(int id, string name, float cost, byte score, string description, float volume, bool hasgase, bool isalcohol) : base(id, name, cost, score, description)
        {
            Volume = volume;
            HasGase = hasgase;
            IsAlcohol = isalcohol;
        }

        protected override string GetInfoWithoutName(string space = "")
        {
            StringBuilder sb = new StringBuilder();
            StringBuilderExtension.Space = space;

            sb.SpaceAppend($"{Sources.Cost}: {Cost:$0.00}" + (Discount > 0 ? $" ({Sources.Discount} {Discount}%)\n" : "\n"));
            if (!String.IsNullOrEmpty(Description))
                sb.SpaceAppend($"{Sources.Description}: {Description}\n");
            sb.SpaceAppend($"{Sources.Volume}: {Volume} {Sources.g}\n");
            sb.SpaceAppend(HasGase ? $"{Sources.WithGase}\n" : $"{Sources.WithoutGase}\n");
            sb.SpaceAppend(IsAlcohol ? $"{Sources.Alcoholic}\n" : $"{Sources.NotAlcoholic}\n");
            sb.SpaceAppend($"{Sources.Score}: {(float)Score / 2}*");

            return sb.ToString();
        }
    }
}
