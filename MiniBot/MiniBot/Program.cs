using MiniBot.Activity;
using MiniBot.Products;
using System;
using System.Data.SqlClient;
using System.IO;
using LogInfo;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Resources;
using System.Globalization;
using System.Threading;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Init();

            AssistantBot bot = new AssistantBot();

            //Console.WindowWidth = 140;
            //Console.WindowHeight = 40;

            //bot.SendMessage("Hi", Sources.BotState.WriteAndWait);
            //bot.SendMessage("Login or register?", Sources.BotState.AccountDecision);
            //bot.DoAction(bot.GetBuffer());
            //bot.SendMessage(null, Sources.BotState.FindAccount);
            //bot.DoAction(bot.GetBuffer());

            while (bot.State != Sources.BotState.Sleep)
            {
                // GetAnswer
                // DoAction
                // SendMessage
            }

            bot.SetLanguage();

            bot.Start();
        }
    }
}
