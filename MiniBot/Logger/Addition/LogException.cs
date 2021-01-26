using System;

namespace LogInfo
{
    class LogException : Exception
    {
        public LogException(string message) : base(message) { }
    }
}
