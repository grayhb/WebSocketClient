using System;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Threading;

namespace WebSocketClient
{
    class Program
    {
        static Parameters _Parameters = null;
        static Logger _Logger = new Logger();

        static bool _NeedRestart = true;
        static DateTime _LastDateTimeSendMsg;

        static Timer _TimerCheckEvents;
        static int _TimerPeriod = 10000;

        static void Main(string[] args)
        {
            if (!LoadServerParameters())
                return;

            _TimerCheckEvents = new Timer(TimerCallback, null, 0, _TimerPeriod);

            _LastDateTimeSendMsg = DateTime.Now;

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
            _Parameters = new Settings().LoadParams();
            return _Parameters != null;
        }

        /// <summary>
        /// Init Callback Timer
        /// </summary>
        /// <param name="o"></param>
        static void TimerCallback(Object o)
        {
            DateTime d = DateTime.Now;

            if (d.Hour == 0 && !_NeedRestart)
            {
                _NeedRestart = true;
                _Logger.AddLog($"Variables updated");
            }
            else if (d.Hour == 5 && _NeedRestart)
            {
                _Logger.AddLog($"Try send command - restart");
                SendRconCmd("restart");
            }

            //выводим сообщение о группе
            TimeSpan span = DateTime.Now.Subtract(_LastDateTimeSendMsg);
                if (span.Hours >= 1)
                {
                _LastDateTimeSendMsg = DateTime.Now;

                string sayMsgs = "";
                foreach (var _msg in _Parameters.LisgMsgs)
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
            using (var ws = new WebSocket(_Parameters.WSAddress))
            {
                ws.OnMessage += (sender, e) =>
                {
                    _Logger.AddLog($"RCON RESPONSE:\n{e.Data}");
                };

                ws.OnError += (sender, e) => {
                    _Logger.AddLog($"RCON ERROR:\n{e.Message}");
                };

                ws.Connect();

                RCONMessage m = new RCONMessage();
                m.Identifier = -1;
                m.Name = "WebRcon";
                m.Message = cmd;

                string s = JsonConvert.SerializeObject(m);

                ws.Send(s);

                _Logger.AddLog($"SEND CMD: {cmd}");

                Thread.Sleep(500);

                ws.Close();
            }

            _NeedRestart = false;
        }

    }
}
