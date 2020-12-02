using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBot.Products
{
    abstract class Product
    {
        public float Cost { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public byte Score { get; set; }

        public Product(string name, float cost, byte score, string description)
        {
            Name = name;
            Cost = cost;
            Score = score;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name}: {Cost:$0.00}";
        }
    }
}
