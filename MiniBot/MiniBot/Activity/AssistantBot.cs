﻿using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static MiniBot.Activity.Sources;
using System.Text.Json;
using System.IO;

namespace MiniBot.Activity
{
    class AssistantBot : IBot
    {
        private enum MenuType : byte
        {
            Pizza,
            Sushi,
            Drink
        }
        private class UserAccount
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
        }

        //private DBWorker _dbWorker = new DBWorker(@"D:\GIT\MYFINALPROJECT\MINIBOT\MINIBOT\RESOURCERS\DBPRODUCTS.MDF");
        private List<string> _choices = new List<string>();
        private UserAccount _account;
        private string _buffer;
        private static string _lastMessage;
        private MenuType _menu;
        public string BotName { get; private set; }
        public BotState State { get; private set; }
        public string Customer { get; private set; } = "Guest";
        public string Indent { get; private set; }

        #region Ctors
        public AssistantBot() : this("Henry") { }

        public AssistantBot(string botname)
        {
            BotName = botname;
            Indent = new String(' ', BotName.Length + 2);
        }
        #endregion

        #region Interface methods
        public void DoAction(string command)
        {
            if (command.Length == 0)
            {
                SendMessage("Please, enter something!");
                return;
            }
            command = command.ToLower();

            if (command[0] == '-' && !IsCommand(command))
            {
                SendMessage("I don't know this command :(!");
                return;
            }

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
                    ExitSystem();
                    break;
                case BotState.Write:
                    break;
                case BotState.WriteAndWait:
                    break;
                case BotState.AccName:
                    if (BackFromAccount(command))
                        break;
                    _account.Name = command;
                    Customer = Char.ToUpper(command[0]) + command.Substring(1);
                    break;
                case BotState.AccLogin:
                    if (BackFromAccount(command))
                        break;
                    if (!command.Contains('@') && !command.Contains('.'))
                    {
                        SendMessage("You should enter your mail. Example: example@example.com");
                        return;
                    }
                    _account.Login = command;
                    break;

                case BotState.AccPassword:
                    if (BackFromAccount(command))
                        break;
                    _account.Password = command;
                    break;
                case BotState.AccountDecision:
                    State = BotState.Write;
                    if (BackFromAccount(command))
                        break;
                    if (Equals(command.Substring(Indent.Length), ChoiceCreate))
                    {
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    else if (Equals(command.Substring(Indent.Length), ChoiceExisting))
                    {
                        State = BotState.FindAccount;
                        SendMessage("Enter your login and password through a whitespace", BotState.FindAccount);
                    }
                    else
                        SendMessage("Sorry, I don't understand you :(", BotState.AccountDecision);
                    break;
                case BotState.FindAccount:
                    if (BackFromAccount(command))
                        break;
                    string[] array = command.Split(' ');
                    if (command.Contains(" ") && array.Length == 2)
                    {
                        if (FindAccount(array[0], array[1]))
                            SendMessage($"Welcome, {Customer}!", BotState.Write);
                        else
                            SendMessage("Incorrect login or password! Try again. To come back enter \"back\"", BotState.FindAccount);
                    }
                    else
                        SendMessage("Bad input value. Enter your login and password throuh a whitespace. Example: \"example@example.com 1234\". To come back enter \"back\"", BotState.FindAccount);
                    break;
                case BotState.AskProduct:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoicePizza:
                            SendMessage("Our menu:", BotState.ShowMenu);
                            _menu = MenuType.Pizza;
                            break;
                        case ChoiceSushi:
                            SendMessage("Our menu:", BotState.ShowMenu);
                            _menu = MenuType.Sushi;
                            break;
                        case ChoiceDrink:
                            SendMessage("Our menu:", BotState.ShowMenu);
                            _menu = MenuType.Drink;
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

        public void GetAnswer()
        {
            if (State == BotState.AccountDecision)
            {
                AddChoice(Indent + ChoiceCreate);
                AddChoice(Indent + ChoiceExisting);

                int index = ChoosePosition();
                while (index < 0)
                {
                    index = ChoosePosition();
                }
                _buffer = _choices[index];
                _choices.Clear();
            }
            else if (State == BotState.ShowMenu)
            {
                switch (_menu)
                {
                    case MenuType.Pizza:
                        break;
                    case MenuType.Sushi:
                        break;
                    case MenuType.Drink:
                        break;
                    default:
                        break;
                }
            }
            else if (State == BotState.AskProduct)
            {
                AddChoice(Indent + ChoicePizza);
                AddChoice(Indent + ChoiceSushi);
                AddChoice(Indent + ChoiceDrink);

                int index = ChoosePosition();
                while (index < 0)
                {
                    index = ChoosePosition();
                }
                _buffer = _choices[index];
                _choices.Clear();
            }
            else if (State != BotState.Sleep && State != BotState.Write)
            {
                WriteBotName(false);
                _buffer = Console.ReadLine();
            }

            DoAction(_buffer);
        }

        public void SendMessage(string msg, BotState nextstate)
        {
            State = nextstate;
            SendMessage(msg);
        }
        #endregion

        #region Private methods area
        private void SendMessage(string msg)
        {
            WriteBotName(true);
            Console.WriteLine(msg);
            _lastMessage = msg;

            GetAnswer();
        }

        private void ExitSystem(int code = 0)
        {
            Console.Write("\nFinishing");

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }

            Console.WriteLine();
            Thread.Sleep(500);

            Environment.Exit(code);
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
            _account = new UserAccount();

            if (State == BotState.AccountDecision)
            {
                SendMessage("Enter your name, please", BotState.AccName);
                SendMessage("Enter your login, please", BotState.AccLogin);
                SendMessage("Enter your password, please", BotState.AccPassword);
                
                SendMessage($"Hooray! Now you have an account and you can buy something.", BotState.Write);
                AddAccount(_account);
            }
            else
            {
                SendMessage("So, may be next time. Good bye!");
                ExitSystem();
            }
        }

        private int ChoosePosition()
        {
            int result = -1;
            (int x, int y) position;
            position = Console.GetCursorPosition();
            int sX = position.x;
            int sY = position.y;

            int temp = _choices.Count - 1;
            Console.SetCursorPosition(position.x, --position.y);
            Console.CursorVisible = false;
            HighLight(_choices[_choices.Count - 1], ConsoleColor.DarkMagenta);

            while (true)
            {
                var cki = Console.ReadKey(true);
                if (temp >= 0 && temp < _choices.Count)
                {
                    HighLight(_choices[temp], ConsoleColor.White);
                }

                if (cki.Key == ConsoleKey.DownArrow && sY - position.y - 1 <= _choices.Count && sY - position.y - 1 > 0)
                {
                    Console.SetCursorPosition(position.x, position.y + 1);
                }
                else if (cki.Key == ConsoleKey.UpArrow && sY - position.y + 1 <= _choices.Count && sY - position.y + 1 > 0)
                {
                    Console.SetCursorPosition(position.x, position.y - 1);
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    result = temp;
                    Console.SetCursorPosition(sX, sY);
                    break;
                }
                else if (cki.Key == ConsoleKey.Escape)
                {
                    Console.CursorVisible = true;
                    Console.SetCursorPosition(sX, sY);
                    return -1;
                }

                position = Console.GetCursorPosition();
                temp = _choices.Count - sY + position.y;

                if (temp >= 0 && temp < _choices.Count)
                {
                    HighLight(_choices[temp], ConsoleColor.DarkMagenta);
                }
            }

            Console.CursorVisible = true;
            return result;
        }

        private void HighLight(string msg, ConsoleColor cc)
        {
            Console.ForegroundColor = cc;
            Console.Write("\r" + msg);
            Console.ResetColor();
        }

        private void AddChoice(string answer)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(answer);
            Console.ResetColor();
            _choices.Add(answer);
        }

        private bool BackFromAccount(string command)
        {
            if (Equals(command, CommandBack))
            {
                SendMessage("Do you want to create a new account or you have the existing one?", BotState.AccountDecision);
                return true;
            }
            return false;
        }
        #endregion

        #region Json Working
        private async void AddAccount(UserAccount account)
        {
            using (FileStream fs = new FileStream("Resources\\accounts.json", FileMode.OpenOrCreate))
            {
                List<UserAccount> accountsList = new List<UserAccount>();
                if (fs.Length != 0)
                    accountsList = await JsonSerializer.DeserializeAsync<List<UserAccount>>(fs);
                accountsList.Add(account);
                fs.SetLength(0);
                await JsonSerializer.SerializeAsync<List<UserAccount>>(fs, accountsList);
            }
        }

        private bool FindAccount(string login, string password)
        {
            List<UserAccount> accountsArray;
            using (StreamReader sr = new StreamReader("Resources\\accounts.json"))
            {
                accountsArray = JsonSerializer.Deserialize<List<UserAccount>>(sr.ReadToEnd());
            }
            UserAccount userAccount = accountsArray.Find(x => Equals(x.Login, login) && Equals(x.Password, password));
            if (userAccount != null)
            {
                Customer = userAccount.Name;
                return true;
            }
            return false;
        }
        #endregion
    }
}
