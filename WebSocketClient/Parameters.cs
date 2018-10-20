using System.Collections.Generic;

namespace WebSocketClient
{
    public class Parameters
    {
        string ip;
        string port;
        string password;

        public string Ip { get { return ip; } set { ip = value; WSAddress = $"ws://{value}"; } }
        public string Port { get { return port; } set { port = value; WSAddress += $":{value}"; } }
        public string Password { get { return password; } set { password = value; WSAddress +=$"/{value}"; } }

        public string WSAddress { get; set; }

        public List<string> LisgMsgs { get; set; }
    }
}
