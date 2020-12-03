using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Activity
{
    class AssistantBot : IBot
    {
        public string BotName { get; private set; } = "Henry";
        public BotState State { get; private set; }
        public string Customer { get; set; }
        
        public void DoAction()
        {
            throw new NotImplementedException();
        }

        public void GetAnswer(string answer)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{BotName}: ");
            Console.ResetColor();
            Console.WriteLine(msg);
            if (State == BotState.WriteAndWait)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{Customer}: ");
                Console.ResetColor();
                Console.ReadLine();
            }
        }
    }
}
