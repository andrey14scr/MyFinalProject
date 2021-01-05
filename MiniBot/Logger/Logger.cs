using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace LogInfo
{
    public enum Mode : byte
    {
        Console = 0,
        File = 1
    }

    public static class Logger
    {
        private static int _counter = 0;

        private const string DBG = "DBG";
        private const string INF = "INF";
        private const string ERR = "ERR";

        private const string folderName = "Logs";

        private const long maxFileSize = 30 * 1024;

        public static Mode Mode { get; set; }
        private static bool _isDebug;
        private static bool _isInited = false;

        public static void Init()
        {
            foreach (var attribute in Assembly.GetExecutingAssembly().GetCustomAttributes(false))
            {
                var debuggableAttribute = attribute as DebuggableAttribute;
                if (debuggableAttribute != null)
                {
                    _isDebug = debuggableAttribute.IsJITTrackingEnabled;
                }
            }

            _isInited = true;
        }

        public static void Info(string message)
        {
            if (!_isInited)
                throw new LogException("a");

            Log(INF, message);
        }

        public static void Debug(string message)
        {
            if (!_isInited)
                throw new LogException("a");

            if (_isDebug)
                Log(DBG, message);
        }

        public static void Error(string message)
        {
            if (!_isInited)
                throw new LogException("a");

            Log(ERR, message);
        }

        private static void Log(string mode, string message)
        {
            StackTrace stackTrace = new StackTrace();
            
            if (Mode == Mode.File)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan timeSpan = nowTime - nowTime.ToUniversalTime();

                string content = nowTime.ToUniversalTime() + " " + 
                    (timeSpan.TotalMinutes >= 0 ? "+" : "-") + timeSpan.ToString("hh\\:mm") + " " +
                    "[" + mode + "] " + message + "\n" + 
                    "Location: " + stackTrace.GetFrame(2).GetMethod().DeclaringType + ", " + stackTrace.GetFrame(2).GetMethod().Name  + "()\n" + 
                    "Thread info. " +
                    "Name: " + (Thread.CurrentThread.Name == null ? "None" : Thread.CurrentThread.Name) + ", " +
                    "priority: " + Thread.CurrentThread.Priority + ", " +
                    "managed ID: " + Thread.CurrentThread.ManagedThreadId + ", " +
                    "state: " + Thread.CurrentThread.ThreadState + ", " +
                    "is alive: " + Thread.CurrentThread.IsAlive + ", " +
                    "is background: " + Thread.CurrentThread.IsBackground + ".\n";


                string fileName = CreateFileName();

                FileInfo file = new FileInfo(fileName);

                if (!file.Exists || file.Exists && file.Length + content.Length * 2 < maxFileSize)
                {
                    WriteInFile(fileName, content);
                }
                else
                {
                    _counter++;
                    fileName = CreateFileName();
                    WriteInFile(fileName, content);
                }
            }
            else
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan timeSpan = nowTime - nowTime.ToUniversalTime();

                Console.Write(nowTime.ToUniversalTime() + " " +
                    (timeSpan.TotalMinutes >= 0 ? "+" : "-") + timeSpan.ToString("hh\\:mm") + " [");

                Console.ForegroundColor = ConsoleColor.Black;
                switch (mode)
                {
                    case DBG:
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        break;
                    case ERR:
                        Console.BackgroundColor = ConsoleColor.Red;
                        break;
                    default:
                        Console.BackgroundColor = ConsoleColor.White;
                        break;
                }
                Console.Write(mode);
                Console.ResetColor();
                Console.WriteLine("] " + message + "\n" +
                    "Location: " + stackTrace.GetFrame(2).GetMethod().DeclaringType + ", " + stackTrace.GetFrame(2).GetMethod().Name + "()\n" +
                    "Thread info. " +
                    "Name: " + (Thread.CurrentThread.Name == null ? "None" : Thread.CurrentThread.Name) + ", " +
                    "priority: " + Thread.CurrentThread.Priority + ", " +
                    "managed ID: " + Thread.CurrentThread.ManagedThreadId + ", " +
                    "state: " + Thread.CurrentThread.ThreadState + ", " +
                    "is alive: " + Thread.CurrentThread.IsAlive + ", " +
                    "is background: " + Thread.CurrentThread.IsBackground + ".\n");
            }
        }

        private static string CreateFileName()
        {
            DateTime nowTime = DateTime.Now;
            return folderName + "\\log_" + nowTime.Year + nowTime.Month + nowTime.Day + "_" + _counter + ".txt";
        }

        private static void WriteInFile(string path, string content)
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            using (var sw = new StreamWriter(path, true))
            {
                sw.WriteLine(content);
            }
        }
    }
}
