using MiniBot.Activity;
using LogInfo;
using System.Net.Mail;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            const short down = 25, up = 24;

            System.Console.WriteLine($"{Sources.GetLocal("Navigation")} {(char)down}, {(char)up}, Enter \n");

            Logger.Init();
            Logger.Debug("In debug mode started");

            AssistantBot bot = new AssistantBot();

            bot.SetLanguage();
            bot.Start();

            System.Console.Read();
        }
    }
}
