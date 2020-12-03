using System;
using MiniBot.Activity;
using MiniBot.Products;

using BS = MiniBot.Activity.Sources.BotState;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            AssistantBot bot = new AssistantBot();
            string whiteSpaces = new String(' ', bot.BotName.Length + 2);
            //string[] str = new string[] { "Cheese", "Meet" };
            //Pizza p = new Pizza("Narodnaya", 22.42f, 9, "great choice", str, 300, 25);
            //Drink d = new Drink("Coca-Cola", 10.5f, 8, "cool drink", 1f, false, false);
            //Sushi s = new Sushi("hiku", 2.1f, 10, "very yammy", str, 12, true);

            bot.SendMessage($"Hello, I am {bot.BotName} - your bot assistant, that can help you to take an order.\n" + 
                whiteSpaces + "If you have some questions, just enter \"help\".\n" + 
                whiteSpaces + "If you want to exit, just enter \"exit\" in any time.\n" + 
                whiteSpaces + "Let's start.");

            if (!bot.FindAccount())
            {
                bot.SendMessage("Oh, I see that you don't have existing account yet. Do you want to create one?\n" +
                    whiteSpaces + $"Answer: {Sources.CommandYes}/{Sources.CommandNo}", BS.CreateAccount);
            }
            else
            {
                bot.SendMessage("Well, I have found some account, do you want to create a new one or use the existing?\n" +
                    whiteSpaces + $"Answer: {Sources.CommandYes}/{Sources.CommandExisting}", BS.AccountDecision);
            }

            
            //bot.Customer = myName;
            bot.SendMessage("Good bye!", BS.Sleep);
        }
    }
}
