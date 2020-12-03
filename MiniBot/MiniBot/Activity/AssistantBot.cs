using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Activity
{
    class AssistantBot : IBot
    {
        private class UserAccount
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
        }

        private UserAccount _account;
        private string _buffer;
        private string _lastMessage;
        public string BotName { get; private set; }
        public BotState State { get; private set; }
        public string Customer { get; set; } = "Guest";

        public AssistantBot() : this("Henry") { }

        public AssistantBot(string botname)
        {
            BotName = botname;
        }

        public void DoAction(string command)
        {
            if (command.Length == 0)
            {
                SendMessage("Please, enter something!");
                return;
            }
            command = command.ToLower();

            if (Equals(command, CommandExit))
            {
                ExitSystem();
            }
            if (Equals(command, CommandHelp))
            {
                WriteHelp();
                SendMessage(_lastMessage);
                return;
            }

            switch (State)
            {
                case BotState.Sleep:
                    break;
                
                case BotState.Write:
                    break;
                
                case BotState.WriteAndWait:
                    break;
                
                case BotState.CreateAccount:
                    State = BotState.Write;
                    if (Equals(command, CommandYes))
                    {
                        CreateAccount();
                    }
                    else if (Equals(command, CommandNo))
                    {
                        State = BotState.Sleep;
                        ExitSystem();
                    }
                    else
                    {
                        SendMessage("Sorry, I don't understand you :(", BotState.CreateAccount);
                    }
                    break;
                
                case BotState.AccName:
                    _account.Name = command;
                    Customer = Char.ToUpper(command[0]) + command.Substring(1);
                    break;
                
                case BotState.AccLogin:
                    if (!command.Contains('@'))
                    {
                        SendMessage("You chould enter your mail", BotState.AccLogin);
                        return;
                    }
                    _account.Login = command;
                    break;
                
                case BotState.AccPassword:
                    _account.Password = command;
                    break;
                
                case BotState.AccountDecision:
                    State = BotState.Write;
                    if (Equals(command, CommandNew))
                    {
                        CreateAccount();
                    }
                    else if (Equals(command, CommandExisting))
                    {
                        State = BotState.Sleep;
                    }
                    else
                    {
                        SendMessage("Sorry, I don't understand you :(", BotState.CreateAccount);
                    }
                    break;
                
                default:
                    break;
            }
        }

        public void GetAnswer()
        {
            WriteBotName(false);
            _buffer = Console.ReadLine();
            DoAction(_buffer);
        }

        public void SendMessage(string msg)
        {
            WriteBotName(true);
            Console.WriteLine(msg);
            _lastMessage = msg;

            if (State != BotState.Write && State != BotState.Sleep)
            {
                GetAnswer();
            }
        }

        public void SendMessage(string msg, BotState nextstate)
        {
            State = nextstate;
            SendMessage(msg);
        }

        public bool FindAccount()
        {
            return false;
        }

        private void ExitSystem()
        {
            WriteBotName(true);
            Console.Write("Exiting");

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }

            Console.WriteLine();
            Thread.Sleep(500);

            SendMessage("Good bye!", BotState.Sleep);
            Environment.Exit(0);
        }

        private void WriteBotName(bool isbot)
        {
            if (isbot)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{BotName}: ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write($"{Customer}: ");
            }
            Console.ResetColor();
        }

        private void WriteHelp()
        {
            WriteBotName(true);
            Console.WriteLine("This is help.");
        }

        private void CreateAccount()
        {
            State = BotState.CreateAccount;
            _account = new UserAccount();

            if (State == BotState.CreateAccount)
            {
                SendMessage("Enter your name, please", BotState.AccName);
                SendMessage("Enter your login, please", BotState.AccLogin);
                SendMessage("Enter your password, please", BotState.AccPassword);

                SendMessage("Hooray! Now you have an account and you can buy something.");
            }
            else
            {
                SendMessage("So, may be next time. Good bye!");
                ExitSystem();
            }
        }
    }
}
