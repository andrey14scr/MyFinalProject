using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Text.Json;

namespace LogInfo
{
    public enum Mode : byte
    {
        Console = 0,
        File = 1
    }

    [DebugMode]
    public class Logger
    {
        private int _counter = 0;

        private const string DBG = "DBG";
        private const string INF = "INF";
        private const string ERR = "ERR";

        private const string folderName = "Logs";
        private const string settingsName = "settings.json";

        private const long maxFileSize = 30 * 1024;

        public Mode Mode { get; set; }
        private bool _isDebug;
        private bool _isInited = false;

        public Logger()
        {
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            DetectDebug();

            if (File.Exists(folderName + "\\" + settingsName)) 
            {
                using (StreamReader sr = new StreamReader(folderName + "\\" + settingsName))
                {
                    _counter = JsonSerializer.Deserialize<int>(sr.ReadToEnd());
                }
            }

            _isInited = true;
        }

        public void Info(string message)
        {
            if (!_isInited)
                throw new LogException("a");

            Log(INF, message);
        }

        public void Debug(string message)
        {
            if (!_isInited)
                throw new LogException("a");

            if (_isDebug)
                Log(DBG, message);
        }

        public void Error(string message)
        {
            if (!_isInited)
                throw new LogException("a");

            Log(ERR, message);
        }

        private void Log(string mode, string message)
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
                    using (StreamWriter sw = new StreamWriter(folderName + "\\" + settingsName))
                    {
                        sw.Write(JsonSerializer.Serialize<int>(_counter));
                    }

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

        private string CreateFileName()
        {
            DateTime nowTime = DateTime.Now;
            return folderName + "\\log_" + nowTime.Year + nowTime.Month + nowTime.Day + "_[" + _counter.ToString("0000") + "].txt";
        }

        private void WriteInFile(string path, string content)
        {
            using (var sw = new StreamWriter(path, true))
            {
                sw.WriteLine(content);
            }
        }

        private void DetectDebug()
        {
            bool debugMode = false;

            var debuggableAttribute = (DebuggableAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(DebuggableAttribute));

            if (debuggableAttribute != null)
            {
                debugMode = debuggableAttribute.IsJITTrackingEnabled;
            }
            
            var customDebuggableAttribute = (DebugModeAttribute)this.GetType().GetCustomAttribute(typeof(DebugModeAttribute));

            if (customDebuggableAttribute != null)
            {
                if (customDebuggableAttribute.IsDebugMode ^ debugMode)
                    _isDebug = false;
                else
                    _isDebug = true;
            }
        }
    }
}
