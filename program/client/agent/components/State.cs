using Butterfly;
using Butterfly.system.objects.main;
using MongoDB.Driver.Core.Events;

namespace Zealot.client.agent
{
    public class State
    {
        public const string NAME = "State";

        private IClient _client;

        /// <summary>
        /// Оповести клиента что соединение установленно.
        /// </summary>
        public IInput I_connection;

        public State(IClient client)
        {
            _client = client;
        }

        public enum Type
        {
            None,

            // Ожидаем входящие сообщение поступившее по SSL 
            // соедининения, получить логин и пароль.
            WriteInitializeData,

            /// <summary>
            /// Создаем прослушку tcp подключения(в Server)
            /// </summary>
            CreatingWaitTcpConnection,

            /// <summary>
            /// Запрашиваем у клиента tcp соединение.
            /// </summary> <summary>
            RequestTCPConnection,

            /// <summary>
            /// Получен ключ по tcp соединению.
            /// </summary> <summary>
            ReceivedTCPKey,

            /// <summary>
            /// Соединение установлено. Оповестим об этом клиента.
            /// </summary> <summary>
            Connection,

            /// <summary>
            /// Загрузим данные для клиента.
            /// </summary> 
            LoadingData,
        }

        public Type CurrentState { private set; get; } = Type.None;

        public bool Change()
        {
            Logger.S_I.To(_client, $"{NAME}:change state. current state {CurrentState}");

            if (CurrentState == Type.None)
            {
                return WriteInitializeData();
            }
            else if (CurrentState == Type.WriteInitializeData)
            {
                return CreatingWaitTcpConnection();
            }
            else if (CurrentState == Type.CreatingWaitTcpConnection)
            {
                return RequestTCPConnection();
            }
            else if (CurrentState == Type.RequestTCPConnection)
            {
                return ReceivedTCPKey();
            }
            else if (CurrentState == Type.ReceivedTCPKey)
            {
                return Connection();
            }
            else if (CurrentState == Type.Connection)
            {
                return LoadingData();
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:extra call");

                _client.destroy();

                return false;
            }
        }


        /// <summary>
        /// Данное состояние вызовется в момент когда придет первое сообщение с логином и паролем. 
        /// </summary>
        /// <returns></returns>
        private bool WriteInitializeData()
        {
            if (CurrentState == Type.None)
            {
                Logger.S_I.To(_client, $"{NAME}:change of state {Type.None} -> {Type.WriteInitializeData}");

                CurrentState = Type.WriteInitializeData;

                return true;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:error changing state to {Type.WriteInitializeData}. current state was expected {Type.None}");

                _client.destroy();

                return false;
            }
        }

        private bool CreatingWaitTcpConnection()
        {
            if (CurrentState == Type.WriteInitializeData)
            {
                Logger.S_I.To(_client, $"{NAME}:change of state {Type.WriteInitializeData} -> {Type.CreatingWaitTcpConnection}");

                CurrentState = Type.CreatingWaitTcpConnection;

                return true;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:error changing state to {Type.CreatingWaitTcpConnection}. current state was expected {Type.WriteInitializeData}");

                _client.destroy();

                return false;
            }
        }

        private bool RequestTCPConnection()
        {
            if (CurrentState == Type.CreatingWaitTcpConnection)
            {
                Logger.S_I.To(_client, $"{NAME}:change of state {Type.CreatingWaitTcpConnection} -> {Type.RequestTCPConnection}");

                CurrentState = Type.RequestTCPConnection;

                return true;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:error changing state to {Type.RequestTCPConnection}. current state was expected {Type.CreatingWaitTcpConnection}");

                _client.destroy();

                return false;
            }
        }

        private bool ReceivedTCPKey()
        {
            if (CurrentState == Type.RequestTCPConnection)
            {
                Logger.S_I.To(_client, $"{NAME}:change of state {Type.RequestTCPConnection} -> {Type.ReceivedTCPKey}");

                CurrentState = Type.ReceivedTCPKey;

                return true;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:error changing state to {Type.ReceivedTCPKey}. current state was expected {Type.RequestTCPConnection}");

                _client.destroy();

                return false;
            }
        }

        private bool Connection()
        {
            if (CurrentState == Type.ReceivedTCPKey)
            {
                Logger.S_I.To(_client, $"{NAME}:change of state {Type.ReceivedTCPKey} -> {Type.Connection}");

                CurrentState = Type.Connection;

                I_connection.To();

                Change();

                return true;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:error changing state to {Type.ReceivedTCPKey}. current state was expected {Type.Connection}");

                _client.destroy();

                return false;
            }
        }

        private bool LoadingData()
        {
            if (CurrentState == Type.Connection)
            {
                Logger.S_I.To(_client, $"{NAME}:change of state {Type.Connection} -> {Type.LoadingData}");

                CurrentState = Type.LoadingData;

                _client.LoadingData();

                return true;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:error changing state to {Type.LoadingData}. current state was expected {Type.Connection}");

                _client.destroy();

                return false;
            }
        }

        public interface IClient : IInformation
        {
            public void LoadingData();
        }
    }
}