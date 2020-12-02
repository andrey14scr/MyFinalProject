using System;
using MiniBot.Products;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Drink d = new Drink("Coca-Cola", 10.5f, 8);

            //Console.WriteLine(d.ToString());
            Console.WriteLine(d.GetInfo());

            Console.WriteLine();

            string[] s = new string[] { "Cheese", "Meet" };
            Pizza p = new Pizza("Narodnaya", 22.42f, 9, s, 300, 25);
            Console.WriteLine(p.GetInfo());
        }
    }
}
