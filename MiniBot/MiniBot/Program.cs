using System;
using MiniBot.Products;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Drink d = new Drink("Coca-Cola", 10.5f, 8, "cool drink", 1f, false, false);

            //Console.WriteLine(d.ToString());
            Console.WriteLine(d.GetInfo());

            Console.WriteLine();

            string[] str = new string[] { "Cheese", "Meet" };
            Pizza p = new Pizza("Narodnaya", 22.42f, 9, "great choice", str, 300, 25);
            Console.WriteLine(p.GetInfo());

            Console.WriteLine();
            Sushi s = new Sushi("hiku", 2.1f, 10, "very yammy", str, 12, true);
            Console.WriteLine(s.GetInfo());
        }
    }
}
