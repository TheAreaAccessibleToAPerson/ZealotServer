using Zealot.client.listen;

namespace Zealot
{
    public sealed class Server<ClientType> : server.Controller<ClientType>
        where ClientType : Butterfly.system.objects.main.Object, new()
    {
        public const string NAME = "server";

        void Construction()
        {
            Logger.S_I.To(this, "start construction ...");
            {
                obj<SSLShield>(SSLShield.NAME);
                obj<TCPShield>(TCPShield.NAME);

                listen_echo_1_2<string, bool, server.ClientData>(BUS.Client.Echo.TRY_GET_DATA)
                    .output_to(TryGetClientData, Event.SERVER_CLIENT_WORK);

                listen_echo_3_1<string, string, Action<bool>, object>(BUS.Client.Echo.WAIT_TCP_CONNECTION)
                    .output_to(CreatingWaitTCPConnection, Event.SERVER_CLIENT_WORK);
            }
            Logger.S_I.To(this, "end construction.");
        }

        void Start()
        {
            Logger.S_I.To(this, "starting ...");
            {
                send_message(ref State.I_initializeSSL, SSLShield.BUS.Server.Message.INITIALIZE);
                send_message(ref State.I_initializeTCP, TCPShield.BUS.Server.Message.INITIALIZE);

                invoke_event(State.Change, Event.SYSTEM);
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
                if ((DB = new(this, Field.DBName, Field.CollectionName)).Initilize())
                {
                    Logger.S_I.To(this, "success configurate initialize client from DB");
                }
                else Logger.S_W.To(this, "error configurate initialize client from DB");
            }
            Logger.S_I.To(this, "end configurate.");
        }

        void Destroyed()
        {
            {
                invoke_event(State.Stopping, Event.SYSTEM);
            }
            Logger.S_I.To(this, "destroyed.");
        }

        void Stop()
        {
            Logger.S_I.To(this, "stopping ...");
            {
            }
            Logger.S_I.To(this, "stop");
        }

        public struct BUS 
        {
            public struct Client 
            {
                public struct Echo 
                {
                    /// <summary>
                    /// Попытка получить данные клиента по логину.
                    /// </summary> <summary>
                    public const string TRY_GET_DATA = NAME + _.s + "TryGetData";

                    /// <summary>
                    /// Ожидает TCP соединение от клиента.
                    /// </summary> <summary>
                    public const string WAIT_TCP_CONNECTION = NAME + _.s + "WaitTCPConnection";
                }
            }
        }
    }
}