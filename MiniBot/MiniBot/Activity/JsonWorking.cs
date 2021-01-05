using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniBot.Activity
{
    partial class AssistantBot : IBot
    {
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
                _account = userAccount;
                return true;
            }
            return false;
        }

        private void CheckJson()
        {
            _hasAccounts = false;
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
                _hasAccounts = true;
            }
        }
    }
}
