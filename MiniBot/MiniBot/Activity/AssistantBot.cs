using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static MiniBot.Activity.Sources;
using MiniBot.Products;
using System.Net.Mail;
using System.IO;

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

            public delegate void CustomerHandler(string email, string message);
            public event CustomerHandler OrderCompleted;
            public event CustomerHandler OrderDelivered;
            public event CustomerHandler OrderPaid;

            public void SendCompleted()
            {
                OrderCompleted?.Invoke(Login, "Order is completed!");
            }

            public void SendDelivered()
            {
                OrderDelivered?.Invoke(Login, "Order is delivered! Don't forget to take it.");
            }

            public void SendPaid()
            {
                OrderPaid?.Invoke(Login, "Order is paid! Thank you.");
            }
        }

        #region Fields
        private static bool _hasAccounts;
        private bool _backToMenu;

        private Basket<Product> _basket = new Basket<Product>();
        private List<short> _listID = new List<short>();
        private List<string> _choices = new List<string>();

        private short _currentID = -1;

        private string _buffer;
        private const string defaultString = null;

        private Product _currentProduct;
        private DBWorker _dbWorker = new DBWorker(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + (new FileInfo(@"..\..\..\Resourcers\DBProducts.mdf")).FullName + ";Integrated Security=True");
        private UserAccount _account = new UserAccount { Name = guestName };
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
                $"{Indent}If you want to exit from the program in the time of input, just enter \"{CommandExit}\".\n" +
                $"{Indent}Or \"{CommandBack}\" to back on one step ago.\n" +
                $"{Indent}Answer something to start.", BotState.Start);
        }

        public void DoAction(string command)
        {
            if (!CheckCommand(ref command))
            {
                return;
            }

            switch (State)
            {
                case BotState.Sleep:
                    ExitSystem();
                    break;
                case BotState.Start:
                    if (_hasAccounts)
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
                    if (BackFromAccount(_buffer))
                    {
                        SendMessage(defaultString, BotState.AccountDecision);
                        break;
                    }
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
                            _currentProduct = (Pizza)_dbWorker.GetById(_currentID);
                            (_currentProduct as Pizza).ShowInfo(Indent);
                            break;
                        case ProductType.Sushi:
                            _currentProduct = (Sushi)_dbWorker.GetById(_currentID);
                            (_currentProduct as Sushi).ShowInfo(Indent);
                            break;
                        case ProductType.Drink:
                            _currentProduct = (Drink)_dbWorker.GetById(_currentID);
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
                    short amount = 0;
                    if (!Int16.TryParse(_buffer, out amount))
                    {
                        SendMessage("Enter an integer number!");
                        break;
                    }
                    _basket.Add(_currentProduct, _currentID , amount);
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
                        default:
                            WriteBotName(true);
                            Console.WriteLine("Information:");

                            _currentProduct = (Product)_dbWorker.GetById(_currentID);
                            _currentProduct.ShowInfo(Indent);
                            
                            SendMessage(defaultString, BotState.ProductInBusket);
                            break;
                    }
                    break;
                case BotState.ProductInBusket:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoiceRemove:
                            _basket.RemoveById(_currentID);
                            SendMessage(defaultString, BotState.ShowBasket);
                            break;
                        case ChoiceReduce:
                            _basket.Remove(_currentProduct, _currentID);
                            SendMessage(defaultString, BotState.ProductInBusket);
                            break;
                        case ChoiceEnlarge:
                            _basket.Add(_currentProduct, _currentID);
                            SendMessage(defaultString, BotState.ProductInBusket);
                            break;
                        case ChoiceBack:
                            SendMessage(defaultString, BotState.ShowBasket);
                            break;
                    }
                    break;
                case BotState.Confirm:
                    switch (command)
                    {
                        case CommandAgree:
                            OrderCompleted(_account.Login, "Completed");
                            OrderDelivered(_account.Login, "Delivered");
                            OrderPaid(_account.Login, "Paid");
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
                if (_hasAccounts)
                    AddChoice(Indent + ChoiceLogin);
                MakeChoice();
            }
            else if (State == BotState.AskProduct)
            {
                AddChoice(Indent + ChoicePizza);
                AddChoice(Indent + ChoiceSushi);
                AddChoice(Indent + ChoiceDrink);
                if (_basket.Count > 0)
                {
                    AddChoice(Indent + _delimiter);
                    AddChoice(Indent + ChoiceSeeBasket);
                }

                MakeChoice();
            }
            else if (State == BotState.ShowMenu)
            {
                _dbWorker.GetFromDB(ProductToString, _currentType);
                AddChoice(Indent + _delimiter); 
                AddChoice(Indent + ChoiceBack);
                if (_basket.Count > 0)
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
                if (_basket.Count == 0)
                {
                    Console.WriteLine(Indent + "<Empty>");
                }
                else
                {
                    for (int i = 0; i < _basket.Count; i++)
                    {
                        AddChoice(Indent + _basket.GetItemInfo(i));
                    }
                }

                AddChoice(Indent + _delimiter);
                AddChoice(Indent + ChoiceBack);
                AddChoice(Indent + ChoiceBuy);

                MakeChoice();
            }
            else if (State == BotState.ProductInBusket)
            {
                AddChoice(Indent + ChoiceEnlarge);
                var item = _basket.GetById(_currentID);
                if (item.amount > 0)
                    AddChoice(Indent + ChoiceReduce);
                AddChoice(Indent + ChoiceRemove);
                AddChoice(Indent + ChoiceBack);

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
                    case BotState.ProductInBusket:
                        msg = _basket.GetItemInfoById(_currentID);
                        break;
                    case BotState.AskAmount:
                        msg = "How many do you want?";
                        break;
                    case BotState.ShowBasket:
                        msg = "Your basket:";
                        break;
                    case BotState.Confirm:
                        msg = $"The total price is {_basket.TotalPrice:$0.00}. Enter \"-agree\" to confirm order.";
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

        private void CreateAccount()
        {
            _account = new UserAccount() { Name = "Guest" };

            if (State == BotState.AccountDecision)
            {
                
                SendMessage(defaultString, BotState.AccName);
                
                if (BackFromAccount(_buffer))
                {
                    SendMessage(defaultString, BotState.AccountDecision);
                    return;
                }

                _account.Name = Char.ToUpper(_buffer[0]) + _buffer.Substring(1);

                SendMessage(defaultString, BotState.AccBirthDate);

                while (true)
                {
                    if (BackFromAccount(_buffer))
                    {
                        SendMessage(defaultString, BotState.AccountDecision);
                        return;
                    }
                    
                    string[] date = _buffer.Split('.');
                    if (date.ToList().Count != 3)
                    {
                        SendMessage("Bad input format! Format is DD.MM.YYYY");
                        continue;
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
                            SendMessage(ex.Message + " Try again.");
                            continue;
                        }
                    }
                    else
                    {
                        SendMessage("Format is DD.MM.YYYY");
                        continue;
                    }
                    
                    break;
                }

                SendMessage(defaultString, BotState.AccLogin);

                while (true)
                {
                    if (BackFromAccount(_buffer))
                        SendMessage(defaultString, BotState.AccountDecision);
                    
                    if (!_buffer.Contains('@') || !_buffer.Contains('.'))
                    {
                        SendMessage("You should enter your mail. Example: example@example.com");
                        continue;
                    }

                    _account.Login = _buffer;
                    break;
                }

                SendMessage(defaultString, BotState.AccPassword);

                if (BackFromAccount(_buffer))
                    SendMessage(defaultString, BotState.AccountDecision);
                _account.Password = _buffer;

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

        private bool CheckCommand(ref string command)
        {
            if (command.Length == 0)
            {
                SendMessage("Please, enter something!");
                return false;
            }

            command = command.ToLower();

            if (command[0] == '-' && !IsCommand(command))
            {
                SendMessage("I don't know this command :(");
                return false;
            }

            if (Equals(command, CommandExit))
                ExitSystem();

            return true;
        }

        private bool BackFromAccount(string command)
        {
            if (Equals(command, CommandBack))
            {
                _account.Name = guestName;
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

        private static void OrderCompleted(string email, string message)
        {
            SendEmail(email, message);
        }

        private static void OrderDelivered(string email, string message)
        {
            SendEmail(email, message);
        }

        private static void OrderPaid(string email, string message)
        {
            SendEmail(email, message);
        }

        private static void SendEmail(string email, string message)
        {
            try
            {
                MailMessage mail = new MailMessage("Andrey14scr@yandex.ru", email);
                mail.Subject = mailTitle;
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