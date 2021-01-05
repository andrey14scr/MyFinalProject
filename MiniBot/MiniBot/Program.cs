using MiniBot.Activity;
using MiniBot.Products;
using System;
using System.Data.SqlClient;
using System.IO;
using LogInfo;
using System.Reflection;
using System.Diagnostics;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            AssistantBot bot = new AssistantBot();

            //string s = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            /*
            string[] arr = { "meet", "cheese" };
            Pizza p1 = new Pizza("pizaa1", 1.50f, 1, "good one1", arr, 100, 11);
            Pizza p2 = new Pizza("pizaa2", 2.50f, 2, "good one2", arr, 200, 22);
            Pizza p3 = new Pizza("pizaa3", 3.50f, 3, "good one3", arr, 300, 33);
            Sushi s = new Sushi("sushi", 3.50f, 3, "good one3", arr, 300, true);

            Basket<Product> bas = new Basket<Product>();
            bas.Add(p1);
            bas.Add(p2);
            bas.Add(p3);
            bas.Add(s);
            bas.Add(p1, 2);

            Customer customer = new Customer();

            bas.ShowSummary();
            */
            //p.ShowShortInfo();

            bot.Start();
        }
    }
}
