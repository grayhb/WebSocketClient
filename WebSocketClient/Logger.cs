using System;
using System.IO;

namespace WebSocketClient
{
    public class Logger
    {
        string _LogFileName { get; set; }
        int _MaxLineLog { get; set; }
        bool _WriteConsole { get; set; }

        /// <summary>
        /// Init logger
        /// </summary>
        /// <param name="LogFileName">File name</param>
        /// <param name="MaxLineLog">Maximum line</param>
        /// <param name="WriteConsole">Send msg in console</param>
        public Logger(string LogFileName = "log.txt", int MaxLineLog = 1000, bool WriteConsole = true)
        {
            _LogFileName = LogFileName;
            _MaxLineLog = MaxLineLog;
            _WriteConsole = WriteConsole;
        }

        /// <summary>
        /// Add message in log
        /// </summary>
        /// <param name="msg"></param>
        public void AddLog(string msg)
        {
            try
            {
                CheckMaxLine();

                File.AppendAllText(_LogFileName, $"{DateTime.Now} {msg}{Environment.NewLine}");

                if (_WriteConsole)
                    Console.WriteLine($"{DateTime.Now} {msg}");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"LOG ERROR [{DateTime.Now}]\n{ex.Message}");
            }
        }

        /// <summary>
        /// Check max line and erase log
        /// </summary>
        void CheckMaxLine()
        {
            try
            {
                string[] AllLineLog = File.ReadAllLines(_LogFileName);
                if (AllLineLog.Length > _MaxLineLog)
                    File.WriteAllText(_LogFileName, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG ERROR [{DateTime.Now}]\n{ex.Message}");
            }
        }

    }
}
