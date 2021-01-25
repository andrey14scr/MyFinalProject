using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
