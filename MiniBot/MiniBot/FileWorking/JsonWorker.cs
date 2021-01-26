using LogInfo;
using MiniBot.Exceptions;
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

            public AccountWorker(string directory, string file)
            {
                _path = directory + "\\" + file;
                if (!Logger.IsInited)
                {
                    Logger.Init();
                }

                CheckJson();
            }

            public void AddAccount(T account)
            {
                if (account == null)
                {
                    throw new NullReferenceException("Null account reference");
                }
                if (FindAccount(account.Login, account.Password, ref account))
                {
                    throw new AccountExistException("Already existed account", account);
                }

                List<T> accountsList = new List<T>();

                using (StreamReader sr = new StreamReader(_path))
                {
                    try
                    {
                        accountsList = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                        accountsList.Add(account);
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
                        Logger.Error(ex.Message);
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
                if (account == null)
                {
                    throw new NullReferenceException("Null account reference");
                }

                List<T> accountsList;
                using (StreamReader sr = new StreamReader(_path))
                {
                    try
                    {
                        accountsList = JsonSerializer.Deserialize<List<T>>(sr.ReadToEnd());
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                        try
                        {
                            AddAccount(account);
                        }
                        catch (AccountExistException aeEx)
                        {
                            Logger.Info(aeEx.Message, aeEx.InnerObject);
                        }
                        return;
                    }
                }

                accountsList[accountsList.FindIndex(acc => Equals(acc.Login, account.Login) && Equals(acc.Password, account.Password))] = account;

                WriteInfo(accountsList);
            }

            public void DeleteAccount(T account)
            {
                if (account == null)
                {
                    throw new NullReferenceException("Null account reference");
                }

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
                    CreateFile(_path);
                }
                else if (!File.Exists(_path))
                {
                    CreateFile(_path);
                }
                else if (File.ReadAllText(_path) != String.Empty)
                {
                    return true;
                }

                return false;
            }

            public bool HasAccount(string login)
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
                        Logger.Error(ex.Message);
                        return false;
                    }
                }

                return accountsList.Exists(acc => Equals(acc.Login, login));
            }

            private void WriteInfo(List<T> accountsList)
            {
                using (StreamWriter sw = new StreamWriter(_path))
                {
                    try
                    {
                        sw.Write(JsonSerializer.Serialize<List<T>>(accountsList));
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message);
                    }
                }
            }
        
            private void CreateFile(string path)
            {
                var accountFile = File.Create(path);
                accountFile.Close();
            }

        }
    }
}
