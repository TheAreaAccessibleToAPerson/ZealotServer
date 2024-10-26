using System.Net.Sockets;

namespace Zealot.client.listen
{
    public sealed class SSLShield : _SSLShield.Controller
    {
        void Construction()
        {
            Logger.S_I.To(this, "start construction ...");
            {
                listen_message<IListen>(BUS.Server.Message.INITIALIZE)
                    .output_to(Initialize);

                listen_impuls(BUS.Content.Impuls.START_LISTENER)
                    .output_to(StartListener);

                listen_impuls(BUS.Content.Impuls.STOP_LISTENER)
                    .output_to(StopListener);
            }
            Logger.S_I.To(this, "end construction.");
        }

        void Start()
        {
            Logger.S_I.To(this, "starting ...");
            {
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

            if (TryGetServerAddress())
            {
                Logger.S_I.To(this, "end configurate.");
            }
            else Logger.S_W.To(this, "configurate exception.");
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
                }
            }
            Logger.S_I.To(this, "stop");
        }


        public struct BUS
        {
            private const string _NAME = _.s + NAME;

            public struct Content
            {
                public struct Impuls
                {
                    /// <summary>
                    /// Сообщает о начале прослушивания входящих подключений.
                    /// </summary>
                    public const string START_LISTENER = _NAME + "StartListener";

                    /// <summary>
                    /// Сообщает об окончании прослушивания входящих подключений.
                    /// </summary>
                    public const string STOP_LISTENER = _NAME + "StopListener";
                }

                public struct Echo { }
            }

            public struct Server
            {
                public struct Message
                {
                    /// <summary>
                    /// Сообщение от сервера в котором он передает реализацию 
                    /// TCPShield.IReceive интерфейса и сообщает о начале создания
                    /// соединения c сервером. После того как произойдет соединение,
                    /// ожидается что через переданый клиентом интерфейс его оповестят
                    /// об окончания данной процедуры, а так же вернут ему способ 
                    /// передачи данных на сервер.
                    /// </summary>
                    public const string INITIALIZE = _NAME + "Initialize";
                }

                public struct Echo { }
            }
        }

        public interface IListen
        {
            public void Listen(Socket client);
            public string GetKey();

            /// <summary>
            /// Сообщает об окончании иниц
            /// </summary>
            public void EndInitialize();
        }

        public interface IConnection
        {
            /// <summary>
            /// Принимает входящие сообщение типа строка. 
            /// </summary>
            public void Send(string message);

            /// <summary>
            /// Принимает входящие сообщение типа массив байтов.
            /// </summary>
            public void Send(byte[] message);

            /// <summary>
            /// Принимает входящие сообщение типа обьект json.
            /// </summary>
            public void Send<JsonType>(JsonType message);
        }

    }
}

