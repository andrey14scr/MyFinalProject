using System;

namespace MiniBot.Exceptions
{
    class AccountExistException : Exception
    {
        public object InnerObject { get; private set; }

        public AccountExistException(string message) : base(message) { }

        public AccountExistException(string message, object obj) : base(message) 
        {
            InnerObject = obj;
        }
    }
}
