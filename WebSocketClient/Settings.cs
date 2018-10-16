using System;
using System.IO;

namespace WebSocketClient
{
    class Settings
    {
        const string configFileName = "config.ini";

        public Parameters LoadParams()
        {
            if (ExistConfigFile())
            {
                Parameters serverParams = new Parameters();
                using (StreamReader reader = new StreamReader(configFileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var param = line.ToLower().Split('=');
                        switch(param[0].Trim())
                        {
                            case "ip":
                                serverParams.ip = param[1].Trim();
                                break;
                            case "port":
                                serverParams.port = param[1].Trim();
                                break;
                            case "password":
                                serverParams.password = param[1].Trim();
                                break;
                        }
                    }
                }

                if (serverParams.ip == null || serverParams.port == null || serverParams.password == null)
                    return null;
                else
                    return serverParams;
            }
            else return null;
        }

        bool ExistConfigFile()
        {
            if (File.Exists(configFileName))
                return true;
            else
            {
                File.Create(configFileName);
                Console.WriteLine("Необходимо внести данные в конфигурационный файл - {0}", configFileName);
                return false;
            }
        }

    }
}
