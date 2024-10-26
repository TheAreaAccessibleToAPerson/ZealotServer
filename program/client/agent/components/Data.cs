using Butterfly;
using Butterfly.system.objects.main;
using Zealot.client.agent.ssl;
using Zealot.client.agent.ssl.read;
using Zealot.server;

namespace Zealot.client.agent
{
    public class Data 
    {
        public const string NAME = "Client";

        private ClientData _value { set; get; } = null;

        private readonly IClient _client;

        public Data(IClient client) => _client = client;

        private ClientAuthorization _authorization;

        /// <summary>
        /// Создать TCP прослушку.
        /// Первым параметром передаем ip адресс подключающегося клиента.
        /// Вторым параметом ключ.
        /// </summary> <summary>
        public Butterfly.IInput I_creatintTCPListener;

        public void Set(ClientData value)
        {
            if (_value == null)
            {
                Logger.S_I.To(_client, $"{NAME}:initialize ClientData");

                _value = value;
            }
            else
            {
                Logger.S_E.To(_client, $"{NAME}:Попытка повторно проинициализировать ClientData");

                _client.destroy();

                return;
            }
        }

        public bool TryGet(out ClientData value)
        {
            value = _value;

            return value != null;
        }

        /// <summary>
        /// Из read подступают данные для авторизации.
        /// </summary>
        public void StartAuthorization(ClientAuthorization value, IReturn<string> @return)
        {
            if (_authorization == null)
            {
                Logger.I.To(_client, $"{NAME}:start authorization");

                _authorization = value;

                @return.To(value.Login);
            }
            else 
            {
                Logger.W.To(_client, $"Попытка запустить повторную авторизацию.");

                _client.destroy();

                return;
            }
        }

        public void EndAuthorization(bool result, ClientData clientData)
        {
            if (_authorization == null)
            {
                Logger.S_E.To(_client, $"В момент завершения авторизации поле _authorization было равно null");

                _client.destroy();

                return;
            }

            if (result)
            {
                if (clientData.Password != null)
                {
                    if (clientData.Password != "")
                    {
                        if (clientData.Password == _authorization.Password)
                        {
                            Console.WriteLine($"{NAME}:Creating tcp listener");
                            // Пароль совподает.
                            // Генирируем ключ для Tcp подключения.
                            // Создаем прослушку tcp соединения, котоое передаст данный ключ.
                            I_creatintTCPListener.To();
                        }
                        else 
                        {
                            Console.WriteLine($"{NAME}Неверный пароль для данного логина.");
                            // Неверный палоль для логина.
                        }
                    }
                    else 
                    {
                        Console.WriteLine($"{NAME}:Был получен пустой логин из Server.");
                        // Вы получили пустой логин из сервера.

                    }
                }
                else 
                {
                    Console.WriteLine($"{NAME}:Был получен парль из Server равный null.");
                    // Вы получили пароль из сервера равный нулл.

                }
            }
            else
            {
                // На сервере нету клиента с таким логином.

            }
        }

        /// <summary>
        /// TCP прослушивание для данного клинта создано.
        /// </summary> <summary>
        private void EndCreatingTcpLisener()
        {
        }

        public interface IClient : IInformation
        {
        }
    }
}