using System;
using System.IO;
using System.Collections.Generic;

namespace WebSocketClient
{
    class Settings
    {
        const string configFileName = "config.ini";

        /// <summary>
        /// Load parameters
        /// </summary>
        /// <returns></returns>
        public Parameters LoadParams()
        {
            if (ExistConfigFile())
            {
                Parameters serverParams = new Parameters();
                serverParams.LisgMsgs = new List<string>();

                using (StreamReader reader = new StreamReader(configFileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var param = line.Split('=');
                        switch(param[0].Trim().ToLower())
                        {
                            case "ip":
                                serverParams.Ip = param[1].Trim();
                                break;
                            case "port":
                                serverParams.Port = param[1].Trim();
                                break;
                            case "password":
                                serverParams.Password = param[1].Trim();
                                break;
                            case "saymsg":
                                serverParams.LisgMsgs.Add(param[1].Trim());
                                break;
                        }
                    }
                }

                if (serverParams.Ip == null || serverParams.Port == null || serverParams.Password == null)
                    return null;
                else
                    return serverParams;
            }
            else return null;
        }

        /// <summary>
        /// Check exist config file
        /// </summary>
        /// <returns></returns>
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
