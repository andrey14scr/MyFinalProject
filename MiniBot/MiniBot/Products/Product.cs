using MiniBot.Interfaces;
using System;
using System.Text;

namespace MiniBot.Products
{
    abstract class Product : IProduct, IShowInfo
    {
        float _cost;
        public int Id { get; private set; }
        public float Cost 
        {
            get
            {
                return _cost * (1 - (float)Discount / 100);
            }
            private set => _cost = value;
        }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public byte Score { get; set; }
        public byte Discount { get; set; }

        public Product(int id, string name, float cost, byte score, string description)
        {
            Id = id;
            Name = name;
            _cost = cost;
            Score = score;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name}: {Cost:$0.00}";
        }

        public void ShowInfo(string space = "", bool isSpacedName = false)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (isSpacedName)
                Console.WriteLine($"{space}{Name}");
            else
                Console.WriteLine($"{Name}");
            Console.ResetColor();
            Console.WriteLine(this.GetInfoWithoutName(space));
        }

        public void ShowShortInfo(string space = "")
        {
            Console.Write($"{space}{Name} {Cost:$0.00}");
        }

        public string GetShortInfo(string space = "")
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(space);
            sb.Append(Name);
            sb.Append($" {Cost:$0.00}");

            return sb.ToString();
        }

        public string GetInfo(string space = "")
        {
            return Name + "\n" + GetInfoWithoutName();
        }

        abstract protected string GetInfoWithoutName(string space = "");
    }
}
