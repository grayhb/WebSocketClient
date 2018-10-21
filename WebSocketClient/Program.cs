using System;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading;

namespace WebSocketClient
{
    class Program
    {
        static Parameters parameters = null;
        static Logger logger = new Logger();

        static bool NeedRestart = true;
        static DateTime LastDateTimeSendMsg;

        static Timer TimerCheckEvents;
        static int TimerPeriod = 10000;

        static void Main(string[] args)
        {
            if (!LoadServerParameters())
                return;

            TimerCheckEvents = new Timer(TimerCallback, null, 0, TimerPeriod);

            LastDateTimeSendMsg = DateTime.Now;

            ConsoleKeyInfo keyinfo;

            do
            {
                keyinfo = Console.ReadKey();
                Console.Clear();
                Console.WriteLine("For exit, press X");
            }
            while (keyinfo.Key != ConsoleKey.X);

        }

        /// <summary>
        /// Load parameters
        /// </summary>
        /// <returns></returns>
        static bool LoadServerParameters()
        {
            parameters = new Settings().LoadParams();
            return parameters != null;
        }

        /// <summary>
        /// Init Callback Timer
        /// </summary>
        /// <param name="o"></param>
        static void TimerCallback(Object o)
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
                LastDateTimeSendMsg = DateTime.Now;

                string sayMsgs = "";
                foreach (var _msg in parameters.LisgMsgs)
                {
                    if (sayMsgs != "") sayMsgs += "<br>";
                    sayMsgs += $"{_msg}";
                }

                SendRconCmd($"say {sayMsgs}");
            }
        }

        /// <summary>
        /// Send RCON command
        /// </summary>
        /// <param name="cmd"></param>
        static void SendRconCmd(string cmd)
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

                RCONMessage m = new RCONMessage();
                m.Identifier = -1;
                m.Name = "WebRcon";
                m.Message = cmd;

                string s = JsonConvert.SerializeObject(m);

                ws.Send(s);

                logger.AddLog($"SEND CMD: {cmd}");

                Thread.Sleep(500);

                ws.Close();
            }
        }

    }
}
