using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static MiniBot.Activity.Sources;
using MiniBot.Products;
using System.Net.Mail;

//andrey14scr@gmail.com 123

namespace MiniBot.Activity
{
    partial class AssistantBot : IBot
    {
        private class UserAccount
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }

            public delegate void CustomerHandler(string topic, string message);
            public event CustomerHandler OrderCompleted;
            public event CustomerHandler OrderDelivered;
            public event CustomerHandler OrderPaid;

            public void SendCompleted()
            {
                OrderCompleted?.Invoke("Your order", "Order is completed!");
            }

            public void SendDelivered()
            {
                OrderDelivered?.Invoke("Your order", "Order is delivered! Don't forget to take it.");
            }

            public void SendPaid()
            {
                OrderPaid?.Invoke("Your order", "Order is paid! Thank you.");
            }
        }

        #region Fields
        private static string _lastMessage;

        private static bool _hasAccounts;
        private bool _backToMenu;

        private Basket<Product> _b = new Basket<Product>();
        private List<short> _listID = new List<short>();
        private List<string> _choices = new List<string>();

        private short _currentID = -1;

        private string _buffer;
        private const string defaultString = null;

        private Product _currentProduct;
        private DBWorker _dbWorker = new DBWorker(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\GIT\MyFinalProject\MiniBot\MiniBot\Resourcers\DBProducts.mdf;Integrated Security=True");
        private UserAccount _account = new UserAccount { Name = "Guest" };
        private ProductType _currentType;
        #endregion

        #region Properties
        public string BotName { get; private set; }
        public BotState State { get; private set; }
        public string Indent { get; private set; }
        #endregion

        #region Ctors
        public AssistantBot() : this("Henry") { }

        public AssistantBot(string botname)
        {
            BotName = botname;
            Indent = new String(' ', BotName.Length + 2);
            CheckJson();
        }
        #endregion

        #region Interface methods
        public void Start()
        { 
            SendMessage($"Hello, I am {BotName} - your bot assistant, that can help you to take an order.\n" +
                $"{Indent}If you have some questions, just enter \"{CommandHelp}\".\n" +
                $"{Indent}If you want to exit, just enter \"{CommandExit}\" in any time.\n" +
                $"{Indent}Answer something to start.", BotState.Start);
        }

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
                SendMessage("I don't know this command :(");
                return;
            }

            if (Equals(command, CommandExit))
                ExitSystem();

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
                case BotState.Start:
                    if (!_hasAccounts)
                    {
                        SendMessage(defaultString, BotState.AccountDecision);
                    }
                    else
                    {
                        SendMessage("Well, to take an order you should have an account. Let's create it.", BotState.Write);
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    break;
                case BotState.AccountDecision:
                    State = BotState.Write;
                    if (BackFromAccount(command))
                        break;
                    if (Equals(command.Substring(Indent.Length), ChoiceRegister))
                    {
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    else if (Equals(command.Substring(Indent.Length), ChoiceLogin))
                    {
                        State = BotState.FindAccount;
                        SendMessage(defaultString, BotState.FindAccount);
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
                        {
                            SendMessage($"Welcome, {_account.Name}! " + defaultString, BotState.AskProduct);
                            SubscribeAccount();
                        }
                        else
                            SendMessage("Incorrect login or password! Try again. To come back enter \"back\"", BotState.FindAccount);
                    }
                    else
                        SendMessage("Bad input value. Enter your login and password throuh a whitespace. Example: \"example@example.com 1234\". To come back enter \"back\"", BotState.FindAccount);
                    break;
                case BotState.AccName:
                    if (BackFromAccount(command))
                        break;
                    _account.Name = Char.ToUpper(command[0]) + command.Substring(1);
                    break;
                case BotState.AccBirthDate:
                    if (BackFromAccount(command))
                        break;
                    string[] date = command.Split('.');
                    if (date.ToList().Count != 3)
                    {
                        SendMessage(defaultString, BotState.AccBirthDate); 
                        break;
                    }
                    int dd = 0, mm = 0, yy = 0;
                    if (Int32.TryParse(date[0], out dd) && Int32.TryParse(date[1], out mm) && Int32.TryParse(date[2], out yy))
                    {
                        try
                        {
                            _account.BirthDate = new DateTime(yy, mm, dd);
                        }
                        catch (Exception ex)
                        {
                            SendMessage(ex.Message + " Try again.", BotState.AccBirthDate);
                            break;
                        }
                    }
                    else
                    {
                        SendMessage("Format is DD.MM.YYYY", BotState.AccBirthDate);
                        break;
                    }
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
                case BotState.AskProduct:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoicePizza:
                            _currentType = ProductType.Pizza;
                            SendMessage(defaultString, BotState.ShowMenu);
                            break;
                        case ChoiceSushi:
                            _currentType = ProductType.Sushi;
                            SendMessage(defaultString, BotState.ShowMenu);
                            break;
                        case ChoiceDrink:
                            _currentType = ProductType.Drink;
                            SendMessage(defaultString, BotState.ShowMenu);
                            break;
                        case ChoiceSeeBasket:
                            _backToMenu = false;
                            SendMessage(defaultString, BotState.ShowBasket);
                            break;
                    }
                    break;
                case BotState.ShowProduct:
                    if (Equals(command.Substring(Indent.Length), ChoiceBack))
                    {
                        SendMessage(defaultString, BotState.AskProduct);
                        break;
                    }
                    else if (Equals(command.Substring(Indent.Length), ChoiceSeeBasket))
                    {
                        _backToMenu = true;
                        SendMessage(defaultString, BotState.ShowBasket);
                        break;
                    }
                    WriteBotName(true);
                    Console.WriteLine("Information:");
                    switch (_currentType)
                    {
                        case ProductType.Pizza:
                            _currentProduct = (Pizza)_dbWorker.GetById(_currentType, _currentID);
                            (_currentProduct as Pizza).ShowInfo(Indent);
                            break;
                        case ProductType.Sushi:
                            _currentProduct = (Sushi)_dbWorker.GetById(_currentType, _currentID);
                            (_currentProduct as Sushi).ShowInfo(Indent);
                            break;
                        case ProductType.Drink:
                            _currentProduct = (Drink)_dbWorker.GetById(_currentType, _currentID);
                            (_currentProduct as Drink).ShowInfo(Indent);
                            break;
                    }
                    SendMessage(defaultString, BotState.ProductDecision);
                    break;
                case BotState.ProductDecision:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoiceBack:
                            SendMessage(defaultString, BotState.ShowMenu);
                            break;
                        case ChoiceTake:
                            SendMessage(defaultString, BotState.AskAmount);
                            break;
                    }
                    break;
                case BotState.AskAmount:
                    _b.Add(_currentProduct, Convert.ToInt16(_buffer));
                    State = BotState.ShowMenu;
                    GetAnswer();
                    break;
                case BotState.ShowBasket:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoiceBack:
                            if (_backToMenu)
                                SendMessage(defaultString, BotState.ShowMenu);
                            else
                                SendMessage(defaultString, BotState.AskProduct);
                            break;
                        case ChoiceBuy:
                            SendMessage(defaultString, BotState.Confirm);
                            break;
                    }
                    break;
                case BotState.Confirm:
                    switch (command)
                    {
                        case CommandAgree:
                            SendMessage(defaultString, BotState.Sleep);
                            break;
                        case CommandBack:
                            SendMessage(defaultString, BotState.ShowBasket);
                            break;
                        case CommandExit:
                            ExitSystem();
                            break;
                        default:
                            SendMessage("I don't understand you :(", BotState.Confirm);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void GetAnswer()
        {
            if (State == BotState.AccountDecision)
            {
                AddChoice(Indent + ChoiceRegister);
                MakeChoice();
            }
            else if (State == BotState.AskProduct)
            {
                AddChoice(Indent + ChoicePizza);
                AddChoice(Indent + ChoiceSushi);
                AddChoice(Indent + ChoiceDrink);
                if (_b.Count > 0)
                {
                    AddChoice(Indent);
                    AddChoice(Indent + ChoiceSeeBasket);
                }

                MakeChoice();
            }
            else if (State == BotState.ShowMenu)
            {
                _dbWorker.GetFromDB(ProductToString, _currentType);
                AddChoice(Indent); 
                AddChoice(Indent + ChoiceBack);
                if (_b.Count > 0)
                    AddChoice(Indent + ChoiceSeeBasket);

                MakeChoice();
                State = BotState.ShowProduct;
            }
            else if (State == BotState.ProductDecision)
            {
                AddChoice(Indent + ChoiceTake);
                AddChoice(Indent + ChoiceBack);

                MakeChoice();
            }
            else if (State == BotState.ShowBasket)
            {
                for (int i = 0; i < _b.Count; i++)
                {
                    AddChoice(Indent + _b.GetItemInfo(i));
                }
                AddChoice(Indent);
                AddChoice(Indent + ChoiceBack);
                AddChoice(Indent + ChoiceBuy);

                MakeChoice();
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
            if (String.IsNullOrEmpty(msg))
            {
                switch (nextstate)
                {
                    case BotState.Sleep:
                        msg = "Thank you";
                        break;
                    case BotState.Start:
                        break;
                    case BotState.Write:
                        break;
                    case BotState.WriteAndWait:
                        break;
                    case BotState.AccName:
                        msg = "Enter your name, please";
                        break;
                    case BotState.AccBirthDate:
                        msg = "Enter your birth date in format DD.MM.YYYY";
                        break;
                    case BotState.AccLogin:
                        msg = "Enter your login, please";
                        break;
                    case BotState.AccPassword:
                        msg = "Enter your password, please";
                        break;
                    case BotState.AccountDecision:
                        msg = "Well, to take an order you should have an account.\n" +
                        $"{Indent}Do you want to register a new account or you can log in the existing one?";
                        break;
                    case BotState.FindAccount:
                        msg = "Enter your login and password through a whitespace";
                        break;
                    case BotState.ShowMenu:
                        msg = "Our menu:";
                        break;
                    case BotState.AskProduct:
                        msg = "What do you want to order?";
                        break;
                    case BotState.ShowProduct:
                        break;
                    case BotState.ProductDecision:
                        msg = "Your decision";
                        break;
                    case BotState.AskAmount:
                        msg = "How many do you want?";
                        break;
                    case BotState.ShowBasket:
                        msg = "Your basket:";
                        break;
                    case BotState.Confirm:
                        msg = $"The total price is {_b.TotalPrice:$0.00}. Enter \"-agree\" to confirm order.";
                        break;
                    default:
                        break;
                }
            }

            State = nextstate;
            SendMessage(msg);
        }
        #endregion

        #region Private methods
        private void SendMessage(string msg)
        {
            WriteBotName(true);
            Console.WriteLine(msg);
            _lastMessage = msg;

            GetAnswer();
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
                Console.Write($"{_account.Name}: ");
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
            _account = new UserAccount() { Name = "Guest" };

            if (State == BotState.AccountDecision)
            {
                SendMessage(defaultString, BotState.AccName);
                SendMessage(defaultString, BotState.AccBirthDate);
                SendMessage(defaultString, BotState.AccLogin);
                SendMessage(defaultString, BotState.AccPassword);
                
                SendMessage("Hooray! Now you have an account and you can buy something.", BotState.Write);
                AddAccount(_account);

                SubscribeAccount();

                SendMessage(defaultString, BotState.AskProduct);
            }
            else
            {
                SendMessage("So, may be next time. Good bye!");
                ExitSystem();
            }
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

        private void ProductToString(Product p, short id)
        {
            AddChoice(Indent + p.ToString());
            _listID.Add(id);
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
        #endregion

        #region EventsWork
        private void SubscribeAccount()
        {
            _account.OrderCompleted += OrderCompleted;
            _account.OrderDelivered += OrderDelivered;
            _account.OrderPaid += OrderPaid;
        }

        private static void OrderCompleted(string topic, string message)
        {
            SendEmail(topic, message);
        }

        private static void OrderDelivered(string topic, string message)
        {
            SendEmail(topic, message);
        }

        private static void OrderPaid(string topic, string message)
        {
            SendEmail(topic, message);
        }

        private static void SendEmail(string topic, string message)
        {
            try
            {
                MailMessage mail = new MailMessage("Andrey14scr@yandex.ru", "andrey14scr@gmail.com");
                mail.Subject = topic;
                mail.Body = message;

                SmtpClient client = new SmtpClient("smtp.yandex.ru");
                client.Port = 25; //587
                client.Credentials = new System.Net.NetworkCredential("Andrey14scr@yandex.ru", "59645206y14");
                client.EnableSsl = true;

                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something was wrong. More: " + ex.Message);
            }
        }
        #endregion
    }
}
