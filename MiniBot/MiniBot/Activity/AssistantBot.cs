using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static MiniBot.Activity.Sources;
using MiniBot.Products;
using System.Net.Mail;
using System.IO;
using LogInfo;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Globalization;
using MiniBot.Exceptions;

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
                OrderCompleted?.Invoke(Login, GetLocal("Order is completed") + "\n" + GetProducts());
            }

            public void SendDelivered(string adress)
            {
                OrderDelivered?.Invoke(Login, GetLocal("Order is delivered") + "\n" + GetLocal("Adress") + ": " + adress);
            }

            public void SendPaid()
            {
                OrderPaid?.Invoke(Login, GetLocal("Order is paid") + "\n" + GetProducts());
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

            private string GetProducts()
            {
                string result = GetLocal("Order") + ":\n";

                for (int i = 0; i < Basket.Count; i++)
                {
                    result += Basket.GetItemInfo(i) + "\n";
                }
                result += "\n";
                result += GetLocal("Total") + $": {Basket.TotalPrice:$0.00}";

                return result;
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
        private DBWorker _dbWorker = new DBWorker(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + (new FileInfo(Environment.CurrentDirectory + @"\Resources\DBProducts.mdf").FullName) + ";Integrated Security=True");
        private UserAccount _account = new UserAccount() { Name = guestName };
        private AccountWorker<UserAccount> accountWorker = new AccountWorker<UserAccount>("Resources", "accounts.json");
        #endregion

        #region Properties
        public string BotName { get; private set; }
        public BotState State { get; private set; }
        public string Indent { get; private set; }
        #endregion

        #region Ctors
        public AssistantBot() : this(GetLocal("Henry"), true) { }

        public AssistantBot(string botname) : this(botname, true) { }

        public AssistantBot(bool isAuto) : this(GetLocal("Henry"), isAuto) { }

        public AssistantBot(string botname, bool isAuto)
        {
            _isAuto = isAuto;
            BotName = botname;
            Indent = new String(' ', BotName.Length + 2);
            accountWorker.CheckJson();

            if (!Logger.IsInited)
                Logger.Init();
        }
        #endregion

        #region Interface methods
        public void Start()
        {
            if (!_isAuto)
                return;

            SendMessage($"{GetLocal("Hello1")} {BotName} {GetLocal("Hello2")}\n" +
                //$"{Indent}If you want to exit from the program in the time of input, just enter \"{CommandExit}\".\n" +
                //$"{Indent}Or \"{CommandBack}\" to back on one step ago.\n" +
                $"{Indent}{GetLocal("Hello3")}", BotState.Start);

            while (State != BotState.Sleep)
            {
                GetAnswer();
            }

            ExitSystem();
        }

        public void DoAction(string command)
        {
            switch (State)
            {
                case BotState.Start:
                    if (accountWorker.CheckJson())
                    {
                        SendMessage(DefaultString, BotState.AccountDecision);
                    }
                    else
                    {
                        SendMessage(GetLocal("AccountAlert"));
                        State = BotState.AccountDecision;
                        try
                        {
                            CreateAccount();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);
                            SendMessage(DefaultString, BotState.Start);
                            return;
                        }
                    }
                    break;
                case BotState.WriteAndWait:
                    GetAnswer();
                    break;
                case BotState.AccountDecision:
                    if (command.Equals(ChoiceRegister))
                    {
                        State = BotState.AccountDecision;
                        try
                        {
                            CreateAccount();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);
                            SendMessage(DefaultString, BotState.Start);
                            return;
                        }
                    }
                    else if (command == ChoiceLogin)
                    {
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
                                    SendMessage($"{GetLocal("Welcome")}, {_account.Name}! " + DefaultString, BotState.AskProduct);
                                    break;
                                }
                                else
                                {
                                    WriteLineMessage(GetLocal("IncorrectLogPas"));
                                    command = ReadMessage();
                                }
                            }
                            else
                            {
                                WriteLineMessage(GetLocal("Incorrect email format"));
                                command = ReadMessage();
                            }
                        }
                        else
                        {
                            WriteLineMessage(GetLocal("Bad input format"));
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

                        try
                        {
                            accountWorker.UpdateAccount(_account);
                        }
                        catch (AccountExistException ex)
                        {
                            Logger.Info(ex.Message, ex.InnerObject);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);
                        }

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
                                typeProduct = GetLocal("pizza");
                                break;
                            case Sushi:
                                typeProduct = GetLocal("sushi");
                                break;
                            case Drink:
                                typeProduct = GetLocal("drink");
                                break;
                            default:
                                break;
                        }

                        typeProduct += " ";
                        WriteMessage(GetLocal("InfoAbout") + " " + typeProduct);
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
                            SendMessage(GetLocal("Can'tBuyAlco"), BotState.Write);
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
                        WriteLineMessage(GetLocal("EnterInt"));
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
                        WriteMessage(GetLocal("InfoAbout") + " ");

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
                            _account.SendDelivered(_adress);
                            SendMessage(DefaultString, BotState.Sleep);
                            break;
                        case CommandBack:
                            SendMessage(DefaultString, BotState.ShowBasket);
                            break;
                        case CommandExit:
                            ExitSystem();
                            break;
                        default:
                            SendMessage(GetLocal("UnknownCommand"), BotState.Confirm);
                            break;
                    }
                    break;
                case BotState.AskAdress:
                    _adress = _buffer;
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
                try
                {
                    ShowProductsChoices(_dbWorker.GetFromDB(x => x.GetType() == _currentType));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }

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
                    Console.WriteLine(Indent + GetLocal("Empty"));
                }
                else
                {
                    for (int i = 0; i < _account.Basket.Count; i++)
                    {
                        try
                        {
                            AddChoice(_account.Basket.GetItemInfo(i));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);
                        }
                    }
                }

                AddChoice(_delimiter);
                if (_account.Basket.Count > 0)
                    AddChoice(ChoiceBuy);
                AddChoice(ChoiceBack);

                MakeChoice();
            }
            else if (State == BotState.ProductInBusket)
            {
                try
                {
                    var item = _account.Basket.GetById(_currentID);
                    if (item.amount > 0)
                    {
                        AddChoice(ChoiceEnlarge);
                        AddChoice(ChoiceReduce);
                        AddChoice(ChoiceRemove);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
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
                        msg = GetLocal("Thank you");
                        break;
                    case BotState.Start:
                        break;
                    case BotState.Write:
                        break;
                    case BotState.WriteAndWait:
                        break;
                    case BotState.AccName:
                        msg = GetLocal("EnterName");
                        break;
                    case BotState.AccBirthDate:
                        msg = GetLocal("EnterBirthDate");
                        break;
                    case BotState.AccLogin:
                        msg = GetLocal("EnterLogin");
                        break;
                    case BotState.AccPassword:
                        msg = GetLocal("EnterPassword");
                        break;
                    case BotState.AccountDecision:
                        msg = GetLocal("ShouldHaveAcc") + "\n" +
                        $"{Indent}{GetLocal("AccDecision")}";
                        break;
                    case BotState.FindAccount:
                        msg = GetLocal("EnterLogPas");
                        break;
                    case BotState.ShowMenu:
                        msg = GetLocal("Our menu") + ":";
                        break;
                    case BotState.AskProduct:
                        msg = GetLocal("WhatToOrder");
                        break;
                    case BotState.ShowProduct:
                        break;
                    case BotState.ProductDecision:
                        msg = GetLocal("Your decision");
                        break;
                    case BotState.ProductInBusket:
                        msg = _account.Basket.GetItemInfoById(_currentID);
                        break;
                    case BotState.AskAmount:
                        msg = GetLocal("HowMany");
                        break;
                    case BotState.ShowBasket:
                        msg = GetLocal("Your basket") + ":";
                        break;
                    case BotState.Confirm:
                        msg = $"{GetLocal("TotalPrice")} {_account.Basket.TotalPrice:$0.00}. {GetLocal("Enter")} \"{CommandAgree}\" {GetLocal("ToConfirm")}.";
                        break;
                    case BotState.AskAdress:
                        msg = GetLocal("EnterAdress");
                        break;
                    default:
                        break;
                }
            }

            State = nextState;

            WriteLineMessage(msg);

            //GetAnswer();
        }
        #endregion

        #region Public methods
        public string GetBuffer()
        {
            return _buffer;
        }

        public void SetLanguage()
        {
            SendMessage(GetLocal("Choose language") + ":");

            AddChoice(EN + "-" + GetLocal("English"));
            AddChoice(RU + "-" + GetLocal("Russian"));

            MakeChoice();

            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(_buffer.Substring(0, 2));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            BotName = GetLocal("Henry");
            _account.Name = GetLocal("Guest");
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
            _account = new UserAccount() { Name = GetLocal("Guest") };

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
                    SendMessage(GetLocal("Bad input format") + " " + GetLocal("DateFormat"));
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
                        Logger.Info(ex.Message);
                        SendMessage(GetLocal("Bad input format") + " " + GetLocal("Try again"));
                        continue;
                    }
                }
                else
                {
                    SendMessage(GetLocal("DateFormat"));
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
                    SendMessage(GetLocal("Incorrect email format"));
                    continue;
                }

                if (accountWorker.HasAccount(_buffer))
                {
                    SendMessage(GetLocal("HasAccount"));
                    continue;
                }

                _account.Login = _buffer;
                break;
            }

            SendMessage(DefaultString, BotState.AccPassword);

            if (BackFromAccount(_buffer))
            {
                SendMessage(DefaultString, BotState.AccountDecision);
                return;
            }
            _account.Password = _buffer;

            SendMessage(GetLocal("AccountCreated"), BotState.Write);

            try
            {
                accountWorker.AddAccount(_account);
            }
            catch (AccountExistException ex)
            {
                Logger.Info(ex.Message, ex.InnerObject);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            SendMessage(DefaultString, BotState.AskProduct);

            SubscribeAccount();
        }

        private bool CheckEmail(string input)
        {
            var emailAttribute = (EmailAttribute)typeof(UserAccount).GetProperty("Login").GetCustomAttributes(false).First(x => x.GetType() == typeof(EmailAttribute));

            if (emailAttribute == null)
            {
                Logger.Error("Email attribute is not found.");
            }

            bool isMatch = false;

            try
            {
                isMatch = Regex.IsMatch(input, emailAttribute.Mask, RegexOptions.IgnoreCase);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }

            return isMatch;
        }

        private bool CheckCommand()
        {
            if (_buffer.Length == 0)
            {
                WriteLineMessage(GetLocal("EnterSmt"));
                WriteBotName(false);
                return false;
            }

            _buffer = _buffer.ToLower();

            if (_buffer.Length > 1 && _buffer[0].Equals('-') && !_buffer[1].Equals('>') && !IsCommand(_buffer) || _buffer.Length == 1 && _buffer[0].Equals('-'))
            {
                WriteLineMessage(GetLocal("UnknownCommand"));
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
            Console.Write("\n" + GetLocal("Finishing"));

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
            //SendEmail(email, message);
            Console.WriteLine(GetLocal("Order is completed"));
        }

        private static void OrderDelivered(string email, string message)
        {
            //SendEmail(email, message);
            Console.WriteLine(GetLocal("Order is delivered"));
        }

        private static void OrderPaid(string email, string message)
        {
            //SendEmail(email, message);
            Console.WriteLine(GetLocal("Order is paid"));
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
                Logger.Error(ex.Message);
            }
        }
        #endregion
    }
}