using MiniBot.Activity;
using LogInfo;

namespace MiniBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Init();

            AssistantBot bot = new AssistantBot();


            bot.SetLanguage();
            bot.Start();
        }
    }
}
