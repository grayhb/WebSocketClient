using System.Collections.Generic;

namespace WebSocketClient
{
    public class Parameters
    {
        string _Ip;
        string _Port;
        string _Password;

        public string Ip { get { return _Ip; } set { _Ip = value; WSAddress = $"ws://{value}"; } }
        public string Port { get { return _Port; } set { _Port = value; WSAddress += $":{value}"; } }
        public string Password { get { return _Password; } set { _Password = value; WSAddress +=$"/{value}"; } }

        public string WSAddress { get; set; }

        public List<string> LisgMsgs { get; set; }
    }
}
