using Butterfly;
using Zealot.client.listen;

namespace Zealot.server
{
    public sealed class State
    {
        public const string NAME = "State";

        public IInput<SSLShield.IListen> I_initializeSSL;
        public IInput<TCPShield.IListen> I_initializeTCP;

        private bool _isStopping = false;

        public enum Type
        {
            None,

            /// <summary>
            /// Загружаем необходимые данные.
            /// </summary>
            LoadingData,

            /// <summary>
            /// Запускает TCP прослушивание клиентов.
            /// </summary>
            ListenTCP,

            /// <summary>
            /// Запускает SSL прослушивание клиентов.
            /// </summary>
            ListenSSL,

            /// <summary>
            /// Все настроено и готово к работе.
            /// </summary> <summary>
            Running,
        }

        private readonly IServer _server;

        public State(IServer server)
        {
            this._server = server;
        }

        public Type CurrentState { private set; get; } = Type.None;

        public void Change()
        {
            if (_isStopping) return;

            Logger.S_I.To(_server, $"{NAME}:change state. current state {CurrentState}");

            if (CurrentState == Type.None)
            {
                LoadingData();
            }
            else if (CurrentState == Type.LoadingData)
            {
                ListenTCP();
            }
            else if (CurrentState == Type.ListenTCP)
            {
                ListenSSL();
            }
            else if (CurrentState == Type.ListenSSL)
            {
                Running();
            }
            else
            {
                Logger.S_E.To(_server, $"{NAME}:extra call");

                _server.Destroy();

                return;
            }
        }

        private void Running()
        {
            if (_isStopping) return;

            if (CurrentState == Type.ListenSSL)
            {
                Logger.S_I.To(_server, $"{NAME}:running");

                CurrentState = Type.Running;
            }
            else
            {
                Logger.S_E.To(_server, $"{NAME}:error changing state to {Type.Running}. current state was expected {Type.ListenSSL}");

                _server.Destroy();

                return;
            }
        }

        public void Stopping()
        {
            if (_isStopping) return;

            Logger.S_I.To(_server, $"{NAME}:stopping");

            _isStopping = true;

            _server.Stopping();
        }

        private void LoadingData()
        {
            if (_isStopping) return;

            if (CurrentState == Type.None)
            {
                Logger.S_I.To(_server, $"{NAME}:change of state {Type.None} -> {Type.LoadingData}");

                CurrentState = Type.LoadingData;

                _server.LoadingData();
            }
            else
            {
                Logger.S_E.To(_server, $"{NAME}:error changing state to {Type.LoadingData}. current state was expected {Type.None}");

                _server.Destroy();

                return;
            }
        }

        private void ListenTCP()
        {
            if (_isStopping) return;

            if (CurrentState == Type.LoadingData)
            {
                Logger.S_I.To(_server, $"{NAME}:change of state {Type.LoadingData} -> {Type.ListenTCP}");

                CurrentState = Type.ListenTCP;

                if (I_initializeTCP != null)
                {
                    Logger.S_I.To(_server, $"{NAME}:to initialize tcp");

                    I_initializeTCP.To(_server);
                }
                else
                {
                    Logger.S_E.To(_server, $"I_initializeTCP is null");

                    _server.Destroy();

                    return;
                }
            }
            else
            {
                Logger.S_E.To(_server, $"{NAME}:error changing state to {Type.ListenTCP}. current state was expected {Type.LoadingData}");

                _server.Destroy();

                return;
            }
        }

        private void ListenSSL()
        {
            if (_isStopping) return;

            if (CurrentState == Type.ListenTCP)
            {
                Logger.S_I.To(_server, $"{NAME}:change of state {Type.ListenTCP} -> {Type.ListenSSL}");

                CurrentState = Type.ListenSSL;

                if (I_initializeSSL != null)
                {
                    Logger.S_I.To(_server, $"{NAME}:to initialize ssl");

                    I_initializeSSL.To(_server);
                }
                else
                {
                    Logger.S_E.To(_server, $"I_initializeSSL is null");

                    _server.Destroy();

                    return;
                }
            }
            else
            {
                Logger.S_E.To(_server, $"{NAME}:error changing state to {Type.ListenSSL}. current state was expected {Type.ListenTCP}");

                _server.Destroy();

                return;
            }
        }
    }
}