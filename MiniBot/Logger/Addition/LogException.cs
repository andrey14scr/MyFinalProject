using System;
using System.Collections.Generic;
using System.Text;

namespace LogInfo
{
    class LogException : Exception
    {
        public LogException(string message) : base(message) { }
    }
}
