using System;
using System.IO;

namespace WebSocketClient
{
    public class Logger
    {
        string LogFileName { get; set; }
        int MaxLineLog { get; set; }
        bool WriteConsole { get; set; }

        /// <summary>
        /// Init logger
        /// </summary>
        /// <param name="LogFileName">File name</param>
        /// <param name="MaxLineLog">Maximum line</param>
        /// <param name="WriteConsole">Send msg in console</param>
        public Logger(string LogFileName = "log.txt", int MaxLineLog = 1000, bool WriteConsole = true)
        {
            this.LogFileName = LogFileName;
            this.MaxLineLog = MaxLineLog;
            this.WriteConsole = WriteConsole;
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

                File.AppendAllText(LogFileName, $"{DateTime.Now} {msg}{Environment.NewLine}");

                if (WriteConsole)
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
                string[] AllLineLog = File.ReadAllLines(LogFileName);
                if (AllLineLog.Length > MaxLineLog)
                    File.WriteAllText(LogFileName, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LOG ERROR [{DateTime.Now}]\n{ex.Message}");
            }
        }

    }
}
