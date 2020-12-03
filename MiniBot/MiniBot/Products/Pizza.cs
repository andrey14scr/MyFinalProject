﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBot.Products
{
    class Pizza : Food
    {
        public byte Size { get; private set; }
        public Pizza(string name, float cost, byte score, string description, string[] ingredients, short weight, byte size) : base(name, cost, score, description, ingredients, weight)
        {
            Size = size;
        }

        private string GetInfo()
        {
            string info = !String.Equals(Description, String.Empty) ? $"Description: {Description}\n" : "";

            return $"Price: {Cost:$0.00}\n" +
                $"Size: {Size}sm\n" +
                $"Weight: {Weight}g\n" +
                info +
                $"Ingredients: {this.GetComposition()}\n" +
                $"Rating: {(float)Score / 2:0.0}*";
        }

        public void WriteInfo()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{Name} ");
            Console.ResetColor();
            Console.WriteLine(GetInfo());
        }
    }
}
