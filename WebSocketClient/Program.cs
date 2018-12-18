using System;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading;

namespace WebSocketClient
{
    class Program
    {
        private static Parameters parameters = null;
        private static Logger logger = new Logger();
        private static bool NeedRestart = true;
        private static DateTime LastDateTimeSendMsg;

        private static Timer TimerCheckEvents;
        private static readonly int TimerPeriod = 10000;

        static void Main(string[] args)
        {
            if (!LoadServerParameters())
                return;

            TimerCheckEvents = new Timer(TimerCallback, null, 0, TimerPeriod);

            LastDateTimeSendMsg = DateTime.Now;

            ConsoleKeyInfo keyinfo;

            do
            {
                Console.WriteLine("For exit, press X");
                Console.WriteLine("For test send messages and command, press T");

                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.T)
                {
                    Console.WriteLine("Try test rcon command:");
                    SendMessage();
                    SendCommand();
                    keyinfo = Console.ReadKey();
                }

                Console.Clear();
                Console.WriteLine("For exit, press X");
            }
            while (keyinfo.Key != ConsoleKey.X);

        }

        /// <summary>
        /// Load parameters
        /// </summary>
        /// <returns></returns>
        private static bool LoadServerParameters()
        {
            parameters = new Settings().LoadParams();
            return parameters != null;
        }

        /// <summary>
        /// Init Callback Timer
        /// </summary>
        /// <param name="o"></param>
        private static void TimerCallback(Object o)
        {
            DateTime d = DateTime.Now;

            if (d.Hour == 0 && !NeedRestart)
            {
                NeedRestart = true;
                logger.AddLog($"Variables updated");
            }
            else if (d.Hour == 5 && NeedRestart)
            {
                logger.AddLog($"Try send command - restart");
                SendRconCmd("restart");
                NeedRestart = false;
            }

            //выводим сообщение о группе
            TimeSpan span = DateTime.Now.Subtract(LastDateTimeSendMsg);
            if (span.Hours >= 1)
            {
                SendMessage();

                SendCommand();
            }
        }

        /// <summary>
        /// Send Message 
        /// </summary>
        private static void SendMessage()
        {
            LastDateTimeSendMsg = DateTime.Now;

            string sayMsgs = "";
            foreach (var msg in parameters.ListMsgs)
            {
                if (sayMsgs != "") sayMsgs += "<br>";
                sayMsgs += $"{msg}";
            }

            SendRconCmd($"say {sayMsgs}");
        }

        /// <summary>
        /// Send Command
        /// </summary>
        private static void SendCommand()
        {
            foreach (var cmd in parameters.ListCommands)
            {
                SendRconCmd(cmd);
            }
        }

        /// <summary>
        /// Send RCON command
        /// </summary>
        /// <param name="cmd"></param>
        private static void SendRconCmd(string cmd)
        {
            using (var ws = new WebSocket(parameters.WSAddress))
            {
                ws.OnMessage += (sender, e) =>
                {
                    logger.AddLog($"RCON RESPONSE:\n{e.Data}");
                };

                ws.OnError += (sender, e) => {
                    logger.AddLog($"RCON ERROR:\n{e.Message}");
                };

                ws.Connect();

                RCONMessage m = new RCONMessage
                {
                    Identifier = -1,
                    Name = "WebRcon",
                    Message = cmd
                };

                string s = JsonConvert.SerializeObject(m);

                ws.Send(s);

                logger.AddLog($"SEND CMD: {cmd}");

                Thread.Sleep(500);

                ws.Close();
            }
        }

    }
}
