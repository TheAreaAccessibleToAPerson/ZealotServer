using Butterfly;

namespace Zealot
{
    public struct _
    {
        public const string s = ":";
    }

    public sealed class Program : Controller, ReadLine.IInformation
    {
        public const string NAME = "program";

        void Construction()
        {
            Logger.S_I.To(this, "start construction ...");
            {
            }
            Logger.S_I.To(this, "end construction.");
        }

        void Start()
        {
            SystemInformation("start");

            Logger.S_I.To(this, "starting ...");
            {
                ReadLine.Start(this);

                obj<Server<client.Agent>>(server.Data.NAME, new server.Setting()
                {
                    EventName = Event.SERVER_CLIENT_WORK,

                    ClientName = client.Agent.NAME,
                    DBName = client.agent.DB.Server.NAME,
                    CollectionName = client.agent.DB.Server.Collection.NAME
                });
            }
            Logger.S_I.To(this, "start.");
        }

        void Destruction()
        {
            Logger.S_I.To(this, "start destruction ...");
            {
            }
            Logger.S_I.To(this, "end destruction.");
        }

        void Configurate()
        {
            Logger.S_I.To(this, "start configurate ...");
            {
                if (MongoDB.DefineConnection("mongodb://localhost:27017", out string defineInfo))
                {
                    Logger.S_I.To(this, defineInfo);

                    if (MongoDB.StartConnection(out string startConnectionInfo))
                    {
                        Logger.S_I.To(this, startConnectionInfo);

                        SystemInformation(startConnectionInfo);
                    }
                    else
                    {
                        Logger.S_I.To(this, startConnectionInfo);

                        SystemInformation(startConnectionInfo);

                        destroy();

                        return;
                    }
                }
                else
                {
                    Logger.S_W.To(this, defineInfo);

                    SystemInformation(defineInfo);

                    destroy();

                    return;
                }
            }
            Logger.S_I.To(this, "end configurate.");
        }

        void Destroyed()
        {
            {
            }
            Logger.S_I.To(this, "destroyed.");
        }

        void Stop()
        {
            Logger.S_I.To(this, "stopping ...");
            {
                if (StateInformation.IsCallStart)
                {
                    ReadLine.Stop(this);
                }
            }
            Logger.S_I.To(this, "stop");
        }

        public void Command(string command)
        {
            if (command == "exit")
            {
                destroy();
            }
            else ReadLine.Input();
        }
    }
}