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
        public byte Score { get; private set; }

        public Product(float cost, string name, string description, byte score)
        {
            Cost = cost;
            Name = name;
            Description = description;
            Score = score;
        }
    }
}
