using System;
using System.Threading;
using MiniBot.Activity;
using MiniBot.Products;

using BS = MiniBot.Activity.Sources.BotState;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //string[] str = new string[] { "Cheese", "Meet" };
            //Pizza p = new Pizza("Narodnaya", 22.42f, 9, "very tasty", str, 300, 25);

            DBWorker dbw = new DBWorker(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\GIT\MyFinalProject\MiniBot\MiniBot\Resourcers\DBProducts.mdf;Integrated Security=True");
            /*
            for (int i = 0; i < 30; i++)
            {
                Pizza p = new Pizza("Pizza " + (i + 1).ToString(), (i+ 2) * 4.5f, (byte)(i % 10), "very tasty", new string[] { i.ToString(), (i+ 18).ToString() }, 300, 25);
                dbw.AddToDB(p);
            }

            for (int i = 0; i < 20; i++)
            {
                Sushi p = new Sushi("Sushi " + (i + 1).ToString(), (i + 1) * 3.4f, (byte)(i % 10), "very fish", new string[] { i.ToString(), (i + 18).ToString() }, 300, i % 2 == 0 ? true : false);
                dbw.AddToDB(p);
            }

            for (int i = 0; i < 10; i++)
            {
                Drink p = new Drink("Drink " + (i + 1).ToString(), (i + 3) * 2.1f, (byte)(i % 10), "very cool", 1.0f, i % 2 == 1 ? true : false, i % 3 == 0 ? true : false);
                dbw.AddToDB(p);
            }
            */

            AssistantBot bot = new AssistantBot();
            string whiteSpaces = new String(' ', bot.BotName.Length + 2);
            
            //Drink d = new Drink("Coca-Cola", 10.5f, 8, "cool drink", 1f, false, false);
            //Sushi s = new Sushi("hiku", 2.1f, 10, "very yammy", str, 12, true);

            bot.SendMessage($"Hello, I am {bot.BotName} - your bot assistant, that can help you to take an order.\n" + 
                $"{bot.Indent}If you have some questions, just enter {Sources.CommandHelp}.\n" +
                $"{bot.Indent}If you want to exit, just enter {Sources.CommandExit} in any time.\n" +
                $"{bot.Indent}Answer something to start.", BS.WriteAndWait);

            bot.SendMessage("Well, to take an order you should have an account.\n" +
                $"{bot.Indent}Do you want to register a new or you can log in the existing one?", BS.AccountDecision);

            bot.SendMessage($"What do you want to order, {bot.Customer}?", BS.AskProduct);


            bot.SendMessage("Good bye!", BS.Sleep);
        }
    }
}
