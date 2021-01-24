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
using LogInfo;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MiniBot.Activity
{
    partial class AssistantBot : IBot
    {
        private class UserAccount : IAccount
        {
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
            [JsonIgnore]
            public Basket<Product> Basket { get; private set; }

            public delegate void CustomerHandler(string email, string message);
            public event CustomerHandler OrderCompleted;
            public event CustomerHandler OrderDelivered;
            public event CustomerHandler OrderPaid;

            public UserAccount()
            {
                Basket = new Basket<Product>();
            }

            public void SendCompleted()
            {
                OrderCompleted?.Invoke(Login, Sources.ResourceManager.GetString("Order is completed", CultureInfo.CurrentCulture));
            }

            public void SendDelivered()
            {
                OrderDelivered?.Invoke(Login, Sources.ResourceManager.GetString("Order is delivered", CultureInfo.CurrentCulture));
            }

            public void SendPaid()
            {
                OrderPaid?.Invoke(Login, Sources.ResourceManager.GetString("Order is paid", CultureInfo.CurrentCulture));
            }

            public void Exit()
            {
                Name = guestName;
                Login = Password = null;
                Basket = null;
                BirthDate = DateTime.MinValue;
            }

            public bool IsAdult()
            {
                return DateTime.Now.Year - this.BirthDate.Year > 18 || (DateTime.Now.Year - this.BirthDate.Year == 18 && DateTime.Now.Month >= this.BirthDate.Month);
            }
        }

        #region Fields
        private bool _backToMenu;
        private bool _isAuto;

        private List<int> _listID = new List<int>();
        private List<string> _choices = new List<string>();

        private int _currentID = -1;

        private string _buffer;
        private const string DefaultString = null;
        private string _adress;

        private Product _currentProduct;
        private Type _currentType;
        private DBWorker _dbWorker = new DBWorker(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + (new FileInfo(@"..\..\..\Resourcers\DBProducts.mdf")).FullName + ";Integrated Security=True");
        private UserAccount _account = new UserAccount() { Name = guestName };
        private AccountWorker<UserAccount> accountWorker = new AccountWorker<UserAccount>("Resources", "accounts.json");
        Logger _logger = new Logger();
        #endregion

        #region Properties
        public string BotName { get; private set; }
        public BotState State { get; private set; }
        public string Indent { get; private set; }
        #endregion

        #region Ctors
        public AssistantBot() : this(Sources.ResourceManager.GetString("Henry", CultureInfo.CurrentCulture), true) { }

        public AssistantBot(string botname) : this(botname, true) { }

        public AssistantBot(bool isAuto) : this(Sources.ResourceManager.GetString("Henry", CultureInfo.CurrentCulture), isAuto) { }

        public AssistantBot(string botname, bool isAuto)
        {
            _isAuto = isAuto;
            BotName = botname;
            Indent = new String(' ', BotName.Length + 2);
            accountWorker.CheckJson();
        }
        #endregion

        #region Interface methods
        public void Start()
        {
            if (!_isAuto)
                return;

            SendMessage($"{Sources.ResourceManager.GetString("Hello1", CultureInfo.CurrentCulture)} {BotName} {Sources.ResourceManager.GetString("Hello2", CultureInfo.CurrentCulture)}\n" +
                //$"{Indent}If you want to exit from the program in the time of input, just enter \"{CommandExit}\".\n" +
                //$"{Indent}Or \"{CommandBack}\" to back on one step ago.\n" +
                $"{Indent}{Sources.ResourceManager.GetString("Hello3", CultureInfo.CurrentCulture)}", BotState.Start);
        }

        public void DoAction(string command)
        {
            switch (State)
            {
                case BotState.Sleep:
                    ExitSystem();
                    break;
                case BotState.Start:
                    if (accountWorker.CheckJson())
                    {
                        SendMessage(DefaultString, BotState.AccountDecision);
                    }
                    else
                    {
                        SendMessage(Sources.ResourceManager.GetString("AccountAlert", CultureInfo.CurrentCulture));
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    break;
                case BotState.WriteAndWait:
                    GetAnswer();
                    break;
                case BotState.AccountDecision:
                    if (command.Equals(ChoiceRegister))
                    {
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    else if (command == ChoiceLogin)
                    {
                        State = BotState.FindAccount;
                        SendMessage(DefaultString, BotState.FindAccount);
                    }
                    break;
                case BotState.FindAccount:
                    while (true)
                    {
                        if (BackFromAccount(_buffer))
                        {
                            SendMessage(DefaultString, BotState.AccountDecision);
                            break;
                        }

                        string[] array = command.Split(' ');
                        if (command.Contains(" ") && array.Length == 2)
                        {
                            var emailAttribute = (EmailAttribute)typeof(UserAccount).GetProperty("Login").GetCustomAttributes(false).First(x => x.GetType() == typeof(EmailAttribute));

                            if (CheckEmail(array[0]))
                            {
                                if (accountWorker.FindAccount(array[0], array[1], ref _account))
                                {
                                    SubscribeAccount();
                                    SendMessage($"Welcome, {_account.Name}! " + DefaultString, BotState.AskProduct);
                                    break;
                                }
                                else
                                {
                                    WriteLineMessage($"Incorrect email or password! Try again.");
                                    command = ReadMessage();
                                }
                            }
                            else
                            {
                                WriteLineMessage($"Incorrect email format!");
                                command = ReadMessage();
                            }
                        }
                        else
                        {
                            WriteLineMessage($"Bad input value. Enter your login and password throuh a whitespace. Example: \"example@example.com 1234\". To come back enter \"{CommandBack}\"");
                            command = ReadMessage();
                        }
                    }
                    break;
                case BotState.AskProduct:
                    if (command.Equals(ChoicePizza))
                    {
                        _currentType = typeof(Pizza);
                        SendMessage(DefaultString, BotState.ShowMenu);
                    }
                    else if (command.Equals(ChoiceSushi))
                    {
                        _currentType = typeof(Sushi);
                        SendMessage(DefaultString, BotState.ShowMenu);
                    }
                    else if (command.Equals(ChoiceDrink))
                    {
                        _currentType = typeof(Drink);
                        SendMessage(DefaultString, BotState.ShowMenu);
                    }
                    else if (command.Equals(ChoiceSeeBasket))
                    {
                        _backToMenu = false;
                        SendMessage(DefaultString, BotState.ShowBasket);
                    }
                    else if (command.Equals(ChoiceExit))
                    {
                        _backToMenu = false;
                        accountWorker.UpdateAccount(_account);
                        _account.Exit();
                        SendMessage(DefaultString, BotState.AccountDecision);
                    }
                    break;
                case BotState.ShowProduct:
                    if (command.Equals(ChoiceBack))
                    {
                        SendMessage(DefaultString, BotState.AskProduct);
                    }
                    else if (command.Equals(ChoiceSeeBasket))
                    {
                        _backToMenu = true;
                        SendMessage(DefaultString, BotState.ShowBasket);
                    }
                    else
                    {
                        string typeProduct = "";
                        _currentProduct = _dbWorker.GetById(_currentID);
                        switch (_currentProduct)
                        {
                            case Pizza:
                                typeProduct = Sources.ResourceManager.GetString("pizza", CultureInfo.CurrentCulture);
                                break;
                            case Sushi:
                                typeProduct = Sources.ResourceManager.GetString("sushi", CultureInfo.CurrentCulture);
                                break;
                            case Drink:
                                typeProduct = Sources.ResourceManager.GetString("drink", CultureInfo.CurrentCulture);
                                break;
                            default:
                                break;
                        }
                        typeProduct += " ";
                        WriteMessage(Sources.ResourceManager.GetString("InfoAbout", CultureInfo.CurrentCulture) + " " + typeProduct);
                        _currentProduct.ShowInfo(Indent);
                        SendMessage(DefaultString, BotState.ProductDecision);
                    }
                    break;
                case BotState.ProductDecision:
                    if (command.Equals(ChoiceBack))
                    {
                        SendMessage(DefaultString, BotState.ShowMenu);
                    }
                    else if (command.Equals(ChoiceTake))
                    {
                        if (_currentProduct is Drink && (_currentProduct as Drink).IsAlcohol && !_account.IsAdult())
                        {
                            SendMessage(Sources.ResourceManager.GetString("Can'tBuyAlco", CultureInfo.CurrentCulture), BotState.Write);
                            SendMessage(DefaultString, BotState.ShowMenu);
                        }
                        else
                            SendMessage(DefaultString, BotState.AskAmount);
                    }
                    break;
                case BotState.AskAmount:
                    short amount = 0;
                    while (!Int16.TryParse(_buffer, out amount))
                    {
                        WriteLineMessage(Sources.ResourceManager.GetString("EnterInt", CultureInfo.CurrentCulture));
                        ReadMessage();
                    }
                    _account.Basket.Add(_currentProduct, _currentID, amount);
                    SendMessage(DefaultString, BotState.ShowMenu);
                    break;
                case BotState.ShowBasket:
                    if (command.Equals(ChoiceBack))
                    {
                        if (_backToMenu)
                            SendMessage(DefaultString, BotState.ShowMenu);
                        else
                            SendMessage(DefaultString, BotState.AskProduct);
                    }
                    else if (command.Equals(ChoiceBuy))
                    {
                        SendMessage(DefaultString, BotState.AskAdress);
                    }
                    else
                    {
                        WriteMessage(Sources.ResourceManager.GetString("InfoAbout", CultureInfo.CurrentCulture) + " ");

                        _currentProduct = _dbWorker.GetById(_currentID);
                        _currentProduct.ShowInfo(Indent);

                        SendMessage(DefaultString, BotState.ProductInBusket);
                    }
                    break;
                case BotState.ProductInBusket:
                    if (command.Equals(ChoiceRemove))
                    {
                        _account.Basket.RemoveById(_currentID);
                        SendMessage(DefaultString, BotState.ShowBasket);
                    }
                    else if (command.Equals(ChoiceReduce))
                    {
                        _account.Basket.Remove(_currentProduct, _currentID);
                        SendMessage(DefaultString, BotState.ProductInBusket);
                    }
                    else if (command.Equals(ChoiceEnlarge))
                    {
                        _account.Basket.Add(_currentProduct, _currentID);
                        SendMessage(DefaultString, BotState.ProductInBusket);
                    }
                    else if (command.Equals(ChoiceBack))
                    {
                        SendMessage(DefaultString, BotState.ShowBasket);
                    }
                    break;
                case BotState.Confirm:
                    switch (command)
                    {
                        case CommandAgree:
                            _account.SendPaid();
                            _account.SendCompleted();
                            _account.SendDelivered();
                            SendMessage(DefaultString, BotState.Sleep);
                            break;
                        case CommandBack:
                            SendMessage(DefaultString, BotState.ShowBasket);
                            break;
                        case CommandExit:
                            ExitSystem();
                            break;
                        default:
                            SendMessage(Sources.ResourceManager.GetString("UnknownCommand", CultureInfo.CurrentCulture), BotState.Confirm);
                            break;
                    }
                    break;
                case BotState.AskAdress:
                    SendMessage(DefaultString, BotState.Confirm);
                    break;
                default:
                    break;
            }
        }

        public void GetAnswer()
        {
            if (State == BotState.AccountDecision)
            {
                AddChoice(ChoiceRegister);
                if (accountWorker.CheckJson())
                    AddChoice(ChoiceLogin);
                MakeChoice();
            }
            else if (State == BotState.AskProduct)
            {
                AddChoice(ChoicePizza);
                AddChoice(ChoiceSushi);
                AddChoice(ChoiceDrink);
                AddChoice(_delimiter);
                if (_account.Basket.Count > 0)
                {
                    AddChoice(ChoiceSeeBasket);
                }
                AddChoice(ChoiceExit);

                MakeChoice();
            }
            else if (State == BotState.ShowMenu)
            {
                ShowProductsChoices(_dbWorker.GetFromDB(x => x.GetType() == _currentType));

                AddChoice(_delimiter);
                if (_account.Basket.Count > 0)
                    AddChoice(ChoiceSeeBasket);
                AddChoice(ChoiceBack);

                MakeChoice();
                State = BotState.ShowProduct;
            }
            else if (State == BotState.ProductDecision)
            {
                AddChoice(ChoiceTake);
                AddChoice(ChoiceBack);

                MakeChoice();
            }
            else if (State == BotState.ShowBasket)
            {
                if (_account.Basket.Count == 0)
                {
                    Console.WriteLine(Indent + Sources.ResourceManager.GetString("Empty", CultureInfo.CurrentCulture));
                }
                else
                {
                    for (int i = 0; i < _account.Basket.Count; i++)
                    {
                        AddChoice(_account.Basket.GetItemInfo(i));
                    }
                }

                AddChoice(_delimiter);
                AddChoice(ChoiceBuy);
                AddChoice(ChoiceBack);

                MakeChoice();
            }
            else if (State == BotState.ProductInBusket)
            {
                AddChoice(ChoiceEnlarge);
                var item = _account.Basket.GetById(_currentID);
                if (item.amount > 0)
                    AddChoice(ChoiceReduce);
                AddChoice(ChoiceRemove);
                AddChoice(ChoiceBack);

                MakeChoice();
            }
            else if (State != BotState.Sleep && State != BotState.Write)
            {
                WriteBotName(false);

                do
                {
                    _buffer = Console.ReadLine();
                } while (!CheckCommand());
            }

            if (_isAuto)
                DoAction(_buffer);
        }

        public void SendMessage(string msg, BotState nextState = BotState.Write)
        {
            if (String.IsNullOrEmpty(msg))
            {
                switch (nextState)
                {
                    case BotState.Sleep:
                        msg = Sources.ResourceManager.GetString("Thank you", CultureInfo.CurrentCulture);
                        break;
                    case BotState.Start:
                        break;
                    case BotState.Write:
                        break;
                    case BotState.WriteAndWait:
                        break;
                    case BotState.AccName:
                        msg = Sources.ResourceManager.GetString("EnterName", CultureInfo.CurrentCulture);
                        break;
                    case BotState.AccBirthDate:
                        msg = Sources.ResourceManager.GetString("EnterBirthDate", CultureInfo.CurrentCulture);
                        break;
                    case BotState.AccLogin:
                        msg = Sources.ResourceManager.GetString("EnterLogin", CultureInfo.CurrentCulture);
                        break;
                    case BotState.AccPassword:
                        msg = Sources.ResourceManager.GetString("EnterPassword", CultureInfo.CurrentCulture);
                        break;
                    case BotState.AccountDecision:
                        msg = Sources.ResourceManager.GetString("ShouldHaveAcc", CultureInfo.CurrentCulture) + "\n" +
                        $"{Indent}{Sources.ResourceManager.GetString("AccDecision", CultureInfo.CurrentCulture)}";
                        break;
                    case BotState.FindAccount:
                        msg = Sources.ResourceManager.GetString("EnterLogPas", CultureInfo.CurrentCulture);
                        break;
                    case BotState.ShowMenu:
                        msg = Sources.ResourceManager.GetString("Our menu", CultureInfo.CurrentCulture) + ":";
                        break;
                    case BotState.AskProduct:
                        msg = Sources.ResourceManager.GetString("WhatToOrder", CultureInfo.CurrentCulture);
                        break;
                    case BotState.ShowProduct:
                        break;
                    case BotState.ProductDecision:
                        msg = Sources.ResourceManager.GetString("Your decision", CultureInfo.CurrentCulture);
                        break;
                    case BotState.ProductInBusket:
                        msg = _account.Basket.GetItemInfoById(_currentID);
                        break;
                    case BotState.AskAmount:
                        msg = Sources.ResourceManager.GetString("HowMany", CultureInfo.CurrentCulture);
                        break;
                    case BotState.ShowBasket:
                        msg = Sources.ResourceManager.GetString("Your basket", CultureInfo.CurrentCulture) + ":";
                        break;
                    case BotState.Confirm:
                        msg = $"{Sources.ResourceManager.GetString("TotalPrice", CultureInfo.CurrentCulture)} {_account.Basket.TotalPrice:$0.00}. {Sources.ResourceManager.GetString("Enter", CultureInfo.CurrentCulture)} \"{Sources.CommandAgree}\" {Sources.ResourceManager.GetString("ToConfirm", CultureInfo.CurrentCulture)}.";
                        break;
                    case BotState.AskAdress:
                        msg = Sources.ResourceManager.GetString("EnterAdress", CultureInfo.CurrentCulture);
                        break;
                    default:
                        break;
                }
            }

            State = nextState;

            WriteLineMessage(msg);

            GetAnswer();
        }
        #endregion

        #region Public methods
        public string GetBuffer()
        {
            return _buffer;
        }
        #endregion

        #region Private methods
        private string ReadMessage()
        {
            WriteBotName(false);
            return _buffer = Console.ReadLine();
        }

        private void WriteLineMessage(string msg, bool isbot = true)
        {
            WriteMessage(msg, isbot);
            Console.WriteLine();
        }

        private void WriteMessage(string msg, bool isbot = true)
        {
            WriteBotName(isbot);
            Console.Write(msg);
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
            _account = new UserAccount() { Name = Sources.ResourceManager.GetString("Guest", CultureInfo.CurrentCulture) };


            SendMessage(DefaultString, BotState.AccName);

            if (BackFromAccount(_buffer))
            {
                SendMessage(DefaultString, BotState.AccountDecision);
                return;
            }

            _account.Name = Char.ToUpper(_buffer[0]) + _buffer.Substring(1);

            while (true)
            {
                SendMessage(DefaultString, BotState.AccBirthDate);

                if (BackFromAccount(_buffer))
                {
                    SendMessage(DefaultString, BotState.AccountDecision);
                    return;
                }

                string[] date = _buffer.Split('.');
                if (date.ToList().Count != 3)
                {
                    SendMessage(Sources.ResourceManager.GetString("Bad input format", CultureInfo.CurrentCulture) + " " + Sources.ResourceManager.GetString("DateFormat", CultureInfo.CurrentCulture));
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
                        SendMessage(ex.Message + " " + Sources.ResourceManager.GetString("Try again", CultureInfo.CurrentCulture));
                        continue;
                    }
                }
                else
                {
                    SendMessage(Sources.ResourceManager.GetString("DateFormat", CultureInfo.CurrentCulture));
                    continue;
                }

                break;
            }

            while (true)
            {
                SendMessage(DefaultString, BotState.AccLogin);

                if (BackFromAccount(_buffer))
                {
                    SendMessage(DefaultString, BotState.AccountDecision);
                    return;
                }

                if (!CheckEmail(_buffer))
                {
                    SendMessage(Sources.ResourceManager.GetString("Incorrect email format", CultureInfo.CurrentCulture));
                    continue;
                }

                _account.Login = _buffer;
                break;
            }

            SendMessage(DefaultString, BotState.AccPassword);

            if (BackFromAccount(_buffer))
                SendMessage(DefaultString, BotState.AccountDecision);
            _account.Password = _buffer;

            SendMessage(Sources.ResourceManager.GetString("AccountCreated", CultureInfo.CurrentCulture), BotState.Write);
            accountWorker.AddAccount(_account);

            SubscribeAccount();

            SendMessage(DefaultString, BotState.AskProduct);
        }

        private bool CheckEmail(string input)
        {
            var emailAttribute = (EmailAttribute)typeof(UserAccount).GetProperty("Login").GetCustomAttributes(false).First(x => x.GetType() == typeof(EmailAttribute));

            if (emailAttribute == null)
            {
                _logger.Error("Email attribute is not found.");
            }

            if (Regex.IsMatch(input, emailAttribute.Mask, RegexOptions.IgnoreCase))
                return true;
            else
                return false;
        }

        private bool CheckCommand()
        {
            if (_buffer.Length == 0)
            {
                WriteLineMessage(Sources.ResourceManager.GetString("EnterSmt", CultureInfo.CurrentCulture));
                WriteBotName(false);
                return false;
            }

            _buffer = _buffer.ToLower();

            if (_buffer.Length > 1 && _buffer[0].Equals('-') && !_buffer[1].Equals('>') && !IsCommand(_buffer) || _buffer.Length == 1 && _buffer[0].Equals('-'))
            {
                WriteLineMessage(Sources.ResourceManager.GetString("UnknownCommand", CultureInfo.CurrentCulture));
                WriteBotName(false);
                return false;
            }

            if (Equals(_buffer, CommandExit))
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

        private void ShowProductsChoices(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                AddChoice(product.ToString());
                _listID.Add(product.Id);
            }
        }

        private void ExitSystem(int code = 0)
        {
            Console.Write("\n" + Sources.ResourceManager.GetString("Finishing", CultureInfo.CurrentCulture));

            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(500);
                Console.Write(".");
            }

            Console.WriteLine();
            Thread.Sleep(500);

            //Environment.Exit(code);
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
            Console.WriteLine(Sources.ResourceManager.GetString("Order is completed", CultureInfo.CurrentCulture));
            //SendEmail(email, message);
        }

        private static void OrderDelivered(string email, string message)
        {
            Console.WriteLine(Sources.ResourceManager.GetString("Order is delivered", CultureInfo.CurrentCulture));
            //SendEmail(email, message);
        }

        private static void OrderPaid(string email, string message)
        {
            Console.WriteLine(Sources.ResourceManager.GetString("Order is paid", CultureInfo.CurrentCulture));
            //SendEmail(email, message);
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