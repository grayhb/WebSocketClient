using System;
using WebSocketSharp;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace WebSocketClient
{
    class Program
    {
        static Parameters ServerParameters = null;

        static bool NeedRestart = true;
        static DateTime LastDateTimeSendMsg;
        static Timer TimerCheckEvents = new Timer(TimerCallback, null, 0, 10000);

        static void Main(string[] args)
        {
            if (!LoadServerParameters())
                return;

            LastDateTimeSendMsg = DateTime.Now;

            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();
                Console.Clear();
                Console.WriteLine("Для завершения программы, нажми X");
            }
            while (keyinfo.Key != ConsoleKey.X);

        }

        /// <summary>
        /// Загрузка параметров из конфиг файла
        /// </summary>
        /// <returns></returns>
        static bool LoadServerParameters()
        {
            ServerParameters = new Settings().LoadParams();
            return ServerParameters != null;
        }

        private static void TimerCallback(Object o)
        {
            DateTime d = DateTime.Now;

            if (d.Hour == 0 && !NeedRestart)
            {
                NeedRestart = true;
                Console.WriteLine(d.ToString() + " Флаг перезагрузки сброшен");
            }
            else if (d.Hour == 5 && NeedRestart)
            {
                Console.WriteLine(d.ToString() + " Время перезагрузить сервер");
                sendCMD("restart");
            }

            //выводим сообщение о группе
            TimeSpan span = DateTime.Now.Subtract(LastDateTimeSendMsg);
            //Console.WriteLine(span.Seconds);
            //if (span.Minutes >= 30)
             if (span.Hours >= 1)
             {
                LastDateTimeSendMsg = DateTime.Now;
                string cmd = "say Друзья! Вступайте в группу ВК(vk.com/rust_manhunt),там все новости по расту и серверу. Приятной игры!";
                sendCMD(cmd);
            }

        }

        static void sendCMD(string cmd)
        {
            string wsAddress = string.Format("ws://{0}:{1}/{2}", ServerParameters.ip, ServerParameters.port, ServerParameters.password);

            using (var ws = new WebSocket(wsAddress))
            {
                ws.OnMessage += (sender, e) =>
                {
                    Console.WriteLine("RCON says: " + e.Data.ToString());
                    AddLog("Пришел ответ: " + e.Data.ToString());
                };

                ws.OnError += (sender, e) => {
                    Console.WriteLine("RCON error: " + e.Message);
                    AddLog("Ошибка: " + e.Message);
                };

                ws.Connect();

                RCONMessage m = new RCONMessage();
                m.Identifier = -1;
                m.Name = "WebRcon";
                m.Message = cmd;

                string s = JsonConvert.SerializeObject(m);

                ws.Send(s);
                Console.WriteLine(DateTime.Now.ToString() + " отправил команду " + cmd + " через RCON");
                AddLog("Пора перезагрузить сервачек, отправил команду " + cmd + " через RCON");

                Thread.Sleep(500);

                ws.Close();
            }

            NeedRestart = false;
        }

        static void AddLog(string msg)
        {
            File.AppendAllText("log.txt",
                   DateTime.Now.ToString() +"   " + msg + Environment.NewLine);
        }

        static void CheckStartUpServer()
        {
            bool flStartApp = false;
            string name = "RustDedicated.exe";
            Process[] pr2 = Process.GetProcesses();
            for (int i = 0; i < pr2.Length; i++)
            {
                if (pr2[i].ProcessName == name)
                {
                    flStartApp = true;
                    break;
                }
            }
            if (!flStartApp)
            {

            }
        }
    }
}
