using LogInfo;

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
        private class AccountWorker<T> where T : IAccount
        {
            private string _path;
            private Logger logger;

            public AccountWorker(string directory, string file)
            {
                _path = directory + "\\" + file;
                logger = new Logger();

                CheckJson();
            }

            public void AddAccount(T account)
            {
                if (FindAccount(account.Login, account.Password, ref account))
                {
                    return;
                }

                List<T> accountsList = new List<T>();

                using (StreamReader sr = new StreamReader("Resources\\accounts.json"))
                {
                    try
                    {
                        accountsList = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                    }
                }

                accountsList.Add(account);
                WriteInfo(accountsList);
            }

            public bool FindAccount(string login, string password, ref T account)
            {
                List<T> accountsList = new List<T>();
                using (StreamReader sr = new StreamReader(_path))
                {
                    try
                    {
                        accountsList = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        return false;
                    }
                }

                T userAccount = accountsList.Find(acc => Equals(acc.Login, login) && Equals(acc.Password, password));
                if (userAccount != null)
                {
                    account = userAccount;
                    return true;
                }
                return false;
            }

            public void UpdateAccount(T account)
            {
                List<T> accountsList;
                using (StreamReader sr = new StreamReader(_path))
                {
                    try
                    {
                        accountsList = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.Message);
                        AddAccount(account);
                        return;
                    }
                }

                accountsList[accountsList.FindIndex(acc => Equals(acc.Login, account.Login) && Equals(acc.Password, account.Password))] = account;

                WriteInfo(accountsList);
            }

            public void DeleteAccount(T account)
            {
                List<T> accountsList;
                using (StreamReader sr = new StreamReader(_path))
                {
                    accountsList = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd());
                }

                accountsList.RemoveAt(accountsList.FindIndex(acc => Equals(acc.Login, account.Login) && Equals(acc.Password, account.Password)));

                WriteInfo(accountsList);
            }

            public bool CheckJson()
            {
                string directory = _path.Split('\\')[0];

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    File.Create(_path);
                }
                else if (!File.Exists(_path))
                {
                    File.Create(_path);
                }
                else if (File.ReadAllText(_path) != String.Empty)
                {
                    return true;
                }

                return false;
            }

            private void WriteInfo(List<T> accountsList)
            {
                using (StreamWriter sw = new StreamWriter(_path))
                {
                    sw.Write(JsonSerializer.Serialize<List<T>>(accountsList));
                }
            }
        }
    }
}
