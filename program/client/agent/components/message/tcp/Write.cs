using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Butterfly;

namespace Zealot.client.agent.tcp
{
    public sealed class Write
    {
        public const string NAME = Agent.NAME + _.s + "TCPWrite";

        private readonly IClient _client;

        private Socket _socket;

        public IInput<byte[]> I_sendSSLBytes;
        public IInput<byte[]> I_sendTCPBytes;

        private bool _isRunning = false;

        public Write(IClient client)
        {
            _client = client;
        }

        public void SetSocket(object socket)
        {
            if (_socket == null)
            {
                Logger.S_I.To(_client, $"Был получен tcp socket");

                _socket = (Socket)socket;
            }
            else 
            {
                Logger.S_I.To(_client, $"Был повторно получили tcp socket");

                _client.destroy();
            }
        }

        public byte[] GetHeader(int type, string str)
        {
            byte[] buffer = new byte[MessageHeader.LENGHT + str.Length];

            int length = Encoding.UTF8.GetBytes(str, 0, str.Length, buffer, MessageHeader.LENGHT);
            {
                buffer[MessageHeader.LENGTH_BYTE_INDEX] = MessageHeader.LENGHT;
                buffer[MessageHeader.TYPE_1BYTE_INDEX] = (byte)(type >> 8);
                buffer[MessageHeader.TYPE_2BYTE_INDEX] = (byte)type;
                buffer[MessageHeader.MESSAGE_LENGTH_1BYTE_INDEX] = (byte)(length >> 24);
                buffer[MessageHeader.MESSAGE_LENGTH_2BYTE_INDEX] = (byte)(length >> 16);
                buffer[MessageHeader.MESSAGE_LENGTH_3BYTE_INDEX] = (byte)(length >> 8);
                buffer[MessageHeader.MESSAGE_LENGTH_4BYTE_INDEX] = (byte)length;
            }

            return buffer;
        }

        public void Send(byte[] value)
        {
            if (_isRunning == false) return;

            try
            {
                // отправляем данные серверу
                _socket.Send(value);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Logger.S_I.To(_client, $"{NAME}:{objectDisposedException}");

                _client.destroy();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Logger.S_I.To(_client, $"{NAME}:{invalidOperationException}");

                _client.destroy();
            }
            catch (IOException ioException)
            {
                Logger.S_I.To(_client, $"{NAME}:{ioException}");

                _client.destroy();
            }
        }

        public void Send(string value)
        {
            if (_isRunning == false) return;

            try
            {
                // конвертируем данные в массив байтов
                byte[] requestData = Encoding.UTF8.GetBytes(value);
                // отправляем данные серверу
                _socket.Send(requestData);
            }
            catch (ObjectDisposedException objectDisposedException)
            {
                Logger.S_I.To(_client, $"{NAME}:{objectDisposedException}");

                _client.destroy();
            }
            catch (InvalidOperationException invalidOperationException)
            {
                Logger.S_I.To(_client, $"{NAME}:{invalidOperationException}");

                _client.destroy();
            }
            catch (IOException ioException)
            {
                Logger.S_I.To(_client, $"{NAME}:{ioException}");

                _client.destroy();
            }
        }

        public static byte[] GetMessageArray(int type, byte[] message)
        {
            int messageLength = message.Length;

            byte[] result = new byte[MessageHeader.LENGHT + messageLength];
            result[MessageHeader.LENGTH_BYTE_INDEX] = MessageHeader.LENGHT;
            result[MessageHeader.TYPE_1BYTE_INDEX] = (byte)(type >> 8);
            result[MessageHeader.TYPE_2BYTE_INDEX] = (byte)type;
            result[MessageHeader.MESSAGE_LENGTH_1BYTE_INDEX] = (byte)(messageLength >> 24);
            result[MessageHeader.MESSAGE_LENGTH_2BYTE_INDEX] = (byte)(messageLength >> 16);
            result[MessageHeader.MESSAGE_LENGTH_3BYTE_INDEX] = (byte)(messageLength >> 8);
            result[MessageHeader.MESSAGE_LENGTH_4BYTE_INDEX] = (byte)messageLength;

            Array.Copy(message, result, MessageHeader.LENGHT);

            return result;
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
            catch (Exception expcetion)
            {
                Logger.S_I.To(_client, $"{NAME}:{expcetion}");

                _client.destroy();
            }
        }

        public bool TryGetRemoveAddress(out string address)
        {
            try
            {
                address = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();

                return true;
            }
            catch (Exception ex)
            {
                address = "";

                Logger.S_W.To(_client, $"TryGetRemoveAddress call. Exception: {ex.ToString()}");

                return false;
            }
        }
    }
}