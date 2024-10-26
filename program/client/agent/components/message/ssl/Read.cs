using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Butterfly;
using Zealot.client.agent.ssl.read;

namespace Zealot.client.agent.ssl
{
    public sealed class Read
    {
        public const string NAME = Agent.NAME + _.s + "SSLRead";

        private readonly IClient _client;

        private Socket _socket;

        private bool _isRunning = false;

        /// <summary>
        /// Обьект реализующий интерфейс IReceive
        /// </summary>
        private readonly IReceive _receive;

        public IInput<ClientAuthorization> I_authorization;

        public Read(IClient client,  object sslClient)
        {
            _client = client;
            _socket = (Socket)sslClient;
        }

        public void Receive()
        {
            if (_isRunning == false) return;

            try
            {
                int length = _socket.Available;

                if (length > 0)
                {
                    byte[] buffer = new byte[length];

                    _socket.Receive(buffer, length, SocketFlags.None);

                    Processing(buffer, length);
                }
            }
            catch (SocketException socketException)
            {
                Logger.S_I.To(_client, $"{NAME}:{socketException}");

                _client.destroy();
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Logger.S_I.To(_client, $"{NAME}:{objectDisposedException}");

                _client.destroy();
            }
        }

        private void Processing(byte[] buffer, int length)
        {
            if (length < 7) return;

            int index = 0;
            int currentStep = 0; int maxStepCount = 1000;
            while (true)
            {
                if (currentStep++ > maxStepCount)
                {
                    Logger.W.To(_client, $"{NAME}:Превышено одновеменое количесво SSL сообщений принятых из сети.");

                    return;
                }

                int headerLength = buffer[index + 0];

                if (headerLength == 7)
                {
                    int type = buffer[index + 1] << 8;
                    type += buffer[index + 2];

                    int messageLength = buffer[index + 3] << 24;
                    messageLength += buffer[index + 4] << 16;
                    messageLength += buffer[index + 5] << 8;
                    messageLength += buffer[index + 6];

                    if (index + 7 + messageLength > length)
                    {
                        Logger.S_W.To(_client, $"{NAME}:Привышена длина сообщения.[Length:{messageLength}]");

                        _client.destroy();

                        return;
                    }

                    // Сообщение с логином и паролем.
                    if (type == read.Type.AUTHORIZATION)
                    {
                        Logger.I.To(_client, $"{NAME}:Message:Authorization");

                        string str = Encoding.UTF8.GetString(buffer, index + 7, messageLength);

                        read.ClientAuthorization authorization = JsonSerializer.Deserialize<read.ClientAuthorization>(str);

                        I_authorization.To(authorization);
                    }
                    else
                    {
                        Logger.S_E.To(_client, $"{NAME}:Неизвестный тип сообщения.");

                        _client.destroy();

                        return;
                    }

                    index += headerLength + messageLength;
                    if (index >= length) return;
                }
                else
                {
                    Logger.S_W.To(_client, $"{NAME}:Пришло сообщение в котором длина заголовка меньше минимально возможной.");

                    _client.destroy();

                    return;
                }
            }
        }

        public void Start()
        {
            Logger.S_I.To(_client, $"{NAME}:Start");

            _isRunning = true;
        }

        /// <summary>
        /// Преостанавливает оправление сообщений.
        /// </summary>
        public void Stop()
        {
            if (_isRunning == false) return;

            Logger.S_I.To(_client, $"{NAME}:Stop");

            _isRunning = false;
        }

        /// <summary>
        /// Отключает
        /// </summary> 
        public void Close()
        {
            if (_isRunning)
            {
                Logger.S_E.To(_client, $"Перед вызовом Close нужно вызвать метод Stop()");

                _client.destroy();

                return;
            }

            if (_socket == null) return;

            try
            {
                _socket.Close();
                _socket = null;
            }
            catch (SocketException expcetion)
            {
                Logger.S_I.To(_client, $"{NAME}:{expcetion}");

                _client.destroy();
            }
        }

        public interface IReceive
        {
            public void Receive(byte[] buffer, int length);
        }
    }
}