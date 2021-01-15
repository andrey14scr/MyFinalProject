using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBot.Products
{
    abstract class Product : IProduct, IShowInfo
    {
        public int Id { get; private set; }
        public float Cost { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public byte Score { get; set; }
        public byte Discount { get; set; }

        public Product(int id, string name, float cost, byte score, string description)
        {
            Id = id;
            Name = name;
            Cost = cost;
            Score = score;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name}: {Cost:$0.00}";
        }

        abstract public void ShowShortInfo(string space = "");

        abstract public void ShowInfo(string space = "");
    }
}
