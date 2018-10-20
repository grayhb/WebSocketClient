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
        /// <param name="logFileName">File name</param>
        /// <param name="maxLineLog">Maximum line</param>
        /// <param name="writeConsole">Send msg in console</param>
        public Logger(string logFileName = "log.txt", int maxLineLog = 1000, bool writeConsole = true)
        {
            this.LogFileName = logFileName;
            this.MaxLineLog = maxLineLog;
            this.WriteConsole = writeConsole;
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
