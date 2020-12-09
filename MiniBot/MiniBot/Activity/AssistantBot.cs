using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static MiniBot.Activity.Sources;
using System.Text.Json;
using System.IO;
using MiniBot.Products;

//andrey14scr@gmail.com 123

namespace MiniBot.Activity
{
    class AssistantBot : IBot, IDisposable
    {
        private class UserAccount
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
        }

        private List<(Product product, byte amount)> _basket = new List<(Product product, byte amount)>();
        private List<short> _listID = new List<short>();
        private short _currentID = -1;
        private Product _currentProduct;
        private DBWorker _dbWorker = new DBWorker(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\GIT\MyFinalProject\MiniBot\MiniBot\Resourcers\DBProducts.mdf;Integrated Security=True");
        private List<string> _choices = new List<string>();
        private UserAccount _account;
        private string _buffer;
        private static string _lastMessage;
        private ProductType _currentType;
        private bool _backToMenu;
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
            CheckJson();
        }
        #endregion

        public void Start()
        {
            SendMessage($"Hello, I am {BotName} - your bot assistant, that can help you to take an order.\n" +
                $"{Indent}If you have some questions, just enter \"{CommandHelp}\".\n" +
                $"{Indent}If you want to exit, just enter \"{CommandExit}\" in any time.\n" +
                $"{Indent}Answer something to start.", BotState.Start);
        }

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
                case BotState.Start:
                    if (HasAccounts)
                    {
                        SendMessage("Well, to take an order you should have an account.\n" +
                        $"{Indent}Do you want to register a new or you can log in the existing one?", BotState.AccountDecision);
                    }
                    else
                    {
                        SendMessage("Well, to take an order you should have an account. Let's create it.", BotState.Write);
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    break;
                case BotState.Write:
                    break;
                case BotState.WriteAndWait:
                    break;
                case BotState.AccName:
                    if (BackFromAccount(command))
                        break;
                    _account.Name = Char.ToUpper(command[0]) + command.Substring(1);
                    Customer = _account.Name;
                    break;
                case BotState.AccBirthDate:
                    if (BackFromAccount(command))
                        break;
                    string[] date = command.Split('.');
                    if (date.ToList().Count != 3)
                    {
                        SendMessage("Format is DD.MM.YYYY", BotState.AccBirthDate); 
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
                    Customer = _account.Name;
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
                    if (Equals(command.Substring(Indent.Length), ChoiceRegister))
                    {
                        State = BotState.AccountDecision;
                        CreateAccount();
                    }
                    else if (Equals(command.Substring(Indent.Length), ChoiceLogin))
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
                            SendMessage($"Welcome, {Customer}! What do you want to order?", BotState.AskProduct);
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
                            SendMessage("Our menu:", BotState.ShowMenu);
                            break;
                        case ChoiceSushi:
                            _currentType = ProductType.Sushi;
                            SendMessage("Our menu:", BotState.ShowMenu);
                            break;
                        case ChoiceDrink:
                            _currentType = ProductType.Drink;
                            SendMessage("Our menu:", BotState.ShowMenu);
                            break;
                        case ChoiceSeeBasket:
                            _backToMenu = false;
                            SendMessage("Your basket:", BotState.ShowBasket);
                            break;
                    }
                    break;
                case BotState.ShowProduct:
                    if (Equals(command.Substring(Indent.Length), ChoiceBack))
                    {
                        SendMessage("What do you want to order?", BotState.AskProduct);
                        break;
                    }
                    else if (Equals(command.Substring(Indent.Length), ChoiceSeeBasket))
                    {
                        _backToMenu = true;
                        SendMessage("Your basket:", BotState.ShowBasket);
                        break;
                    }
                    WriteBotName(true);
                    Console.WriteLine("Information:");
                    switch (_currentType)
                    {
                        case ProductType.Pizza:
                            _currentProduct = (Pizza)_dbWorker.GetById(_currentType, _currentID);
                            (_currentProduct as Pizza).WriteInfo(Indent);
                            break;
                        case ProductType.Sushi:
                            _currentProduct = (Sushi)_dbWorker.GetById(_currentType, _currentID);
                            (_currentProduct as Sushi).WriteInfo(Indent);
                            break;
                        case ProductType.Drink:
                            _currentProduct = (Drink)_dbWorker.GetById(_currentType, _currentID);
                            (_currentProduct as Drink).WriteInfo(Indent);
                            break;
                    }
                    SendMessage("Your decision", BotState.ProductDecision);
                    break;
                case BotState.ProductDecision:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoiceBack:
                            SendMessage("Our menu:", BotState.ShowMenu);
                            break;
                        case ChoiceTake:
                            SendMessage("How many do you want?", BotState.AskAmount);
                            break;
                    }
                    break;
                case BotState.AskAmount:
                    State = BotState.ShowMenu;
                    GetAnswer();
                    break;
                case BotState.ShowBasket:
                    switch (command.Substring(Indent.Length))
                    {
                        case ChoiceBack:
                            if (_backToMenu)
                                SendMessage("Our menu:", BotState.ShowMenu);
                            else
                                SendMessage("What do you want to order?", BotState.AskProduct);
                            break;
                        case ChoiceBuy:
                            SendMessage($"The total price is {TotalPrice():$0.00}. Enter \"-agree\" to confirm order.", BotState.Confirm);
                            break;
                    }
                    break;
                case BotState.Confirm:
                    switch (command)
                    {
                        case CommandAgree:
                            SendMessage("Thank you", BotState.Sleep);
                            break;
                        case CommandBack:
                            SendMessage("Your basket:", BotState.ShowBasket);
                            break;
                        case CommandExit:
                            ExitSystem();
                            break;
                        default:
                            SendMessage("I don't understand you :(", BotState.Confirm);
                            break;
                    }
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
                if (_basket.Count > 0)
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
                foreach (var item in _basket)
                {
                    AddChoice(Indent + item.amount + "x " + item.product.ToString());
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
                if (State == BotState.AskAmount)
                    _basket.Add((_currentProduct, Convert.ToByte(_buffer)));
            }

            DoAction(_buffer);
        }

        public void SendMessage(string msg, BotState nextstate)
        {
            if (Equals(msg, AutoText))
            {
                switch (nextstate)
                {
                    case BotState.Sleep:
                        break;
                    case BotState.Start:
                        break;
                    case BotState.Write:
                        break;
                    case BotState.WriteAndWait:
                        break;
                    case BotState.AccName:
                        break;
                    case BotState.AccBirthDate:
                        break;
                    case BotState.AccLogin:
                        break;
                    case BotState.AccPassword:
                        break;
                    case BotState.AccountDecision:
                        break;
                    case BotState.FindAccount:
                        break;
                    case BotState.ShowMenu:
                        break;
                    case BotState.AskProduct:
                        break;
                    case BotState.ShowProduct:
                        break;
                    case BotState.ProductDecision:
                        break;
                    case BotState.AskAmount:
                        break;
                    case BotState.ShowBasket:
                        break;
                    case BotState.Confirm:
                        break;
                    default:
                        break;
                }
            }

            State = nextstate;
            SendMessage(msg);
        }

        public void Dispose()
        {
            
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
                SendMessage("Enter your birth date in format DD.MM.YYYY", BotState.AccBirthDate);
                SendMessage("Enter your login, please", BotState.AccLogin);
                SendMessage("Enter your password, please", BotState.AccPassword);
                
                SendMessage("Hooray! Now you have an account and you can buy something.", BotState.Write);
                AddAccount(_account);
                SendMessage("What do you want to order?", BotState.AskProduct);
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
                    if (_choices[temp + 1] != null && Equals(_choices[temp + 1], Indent))
                        position.y++;
                    Console.SetCursorPosition(position.x, position.y + 1);
                }
                else if (cki.Key == ConsoleKey.UpArrow && sY - position.y + 1 <= _choices.Count && sY - position.y + 1 > 0)
                {
                    if (_choices[temp - 1] != null && Equals(_choices[temp - 1], Indent))
                        position.y--;
                    Console.SetCursorPosition(position.x, position.y - 1);
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    if (!Equals(_choices[temp], Indent))
                    {
                        result = temp;
                        Console.SetCursorPosition(sX, sY);
                        break;
                    }
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

        private void ProductToString(Product p, short id)
        {
            AddChoice(Indent + p.ToString());
            _listID.Add(id);
        }

        private void MakeChoice()
        {
            int index = ChoosePosition();
            while (index < 0)
            {
                index = ChoosePosition();
            }
            _buffer = _choices[index];

            WriteBotName(false);
            if (State == BotState.ShowMenu && index < _listID.Count)
                _currentID = _listID[index];
            _listID.Clear();
            
            Console.WriteLine(_choices[index].Substring(Indent.Length));

            _choices.Clear();
        }

        private float TotalPrice()
        {
            float price = 0.0f;
            foreach (var item in _basket)
            {
                price += item.product.Cost * item.amount;
            }

            return price;
        }
        #endregion

        #region Json Working
        private async void AddAccount(UserAccount account)
        {
            CheckJson();

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
            CheckJson();

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

        private void CheckJson()
        {
            Sources.HasAccounts = false;
            if (!Directory.Exists("Resources"))
            {
                Directory.CreateDirectory("Resources");
                File.Create("Resources\\accounts.json");
            }
            else if (!File.Exists("Resources\\accounts.json"))
            {
                File.Create("Resources\\accounts.json");
            }
            else if (File.ReadAllText("Resources\\accounts.json") != String.Empty)
            {
                Sources.HasAccounts = true;
            }
        }
        #endregion
    }
}
