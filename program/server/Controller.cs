using System.Net.Sockets;
using Butterfly;
using Zealot.client.listen;

namespace Zealot.server
{
    public abstract class Controller<ClientType> : Controller.LocalField<Setting>, IServer,
        SSLShield.IListen, TCPShield.IListen
        where ClientType : Butterfly.system.objects.main.Object, new()
    {
        private Dictionary<string, ClientData> _clientsData = new();

        /// <summary>
        /// Хранит данные для новых tcp соединений.
        /// </summary>
        private readonly List<TCPConnection> _waitTCPConnections = new();

        protected readonly State State;
        private bool _isRunning = false;

        public Controller()
        {
            State = new(this);
        }

        protected DB DB { set; get; }

        void IServer.LoadingData()
        {
            if (try_fly(() =>
            {
                Logger.S_I.To(this, "loading data ...");

                _clientsData.Add("login", new ClientData()
                {
                    Login = "login",
                    Password = "password"
                });

                State.Change();
            }))
            {
                Logger.S_I.To(this, "LoadingData call");
            }
            else Logger.S_I.To(this, "LoadingData don't call");
        }

        void SSLShield.IListen.Listen(Socket value)
        {
            invoke_event(() =>
            {
                if (try_fly(() =>
                {
                    if (ServerHellper.GetAddressAndPort(value, out string address, out int port, out string info))
                    {
                        Logger.S_I.To(this, $"New ssl client Address:{address}, Port:{port}");

                        string key = address + port;

                        Logger.S_I.To(this, $"creating object {Field.ClientName}[Key:{key}]");

                        if (try_obj(key, out ClientType client))
                        {
                            Logger.S_W.To(this, $"object {Field.ClientName}[Key:{key}] is create then do destroyed and creating new object.");

                            client.destroy();
                        }

                        obj<ClientType>(key, new client.Setting()
                        {
                            Address = address,
                            Port = port,
                            TcpClient = value
                        });
                    }
                    // Если что то просто потеряем нового клиента.
                    else Logger.S_W.To(this, info);
                }))
                {
                    Logger.S_I.To(this, $"SSLShield.IListen.Listen call success");
                }
                else Logger.S_I.To(this, $"SSLShield.IListen.Listen don't call");
            },
            Event.SERVER_CLIENT_WORK);
        }

        void SSLShield.IListen.EndInitialize()
        {
            Logger.S_I.To(this, "SSLShield end initilize");

            invoke_event(State.Change, Event.SYSTEM);
        }

        /// <summary>
        /// Получаем tcp соединение и пытаемся считать данные.
        /// Если данные не будут получены через определеный промежуток времени, 
        /// То сообщим клиенту.
        /// </summary>
        void TCPShield.IListen.Listen(Socket value)
        {
            invoke_event(() =>
            {
                if (ServerHellper.GetAddressAndPort(value, out string address, out int port, out string info))
                {
                    if (try_fly(() =>
                    {
                        string objKey = $"ReceiveTCPKey:{address}:{port}";
                        if (try_obj(objKey, out ReceiveTCPKey obj))
                        {
                            Logger.S_I.To(this, $"Уже ожидается ключ от {address}:{port}. Данное соединение будет проигнарировано.");

                            return;
                        }
                        else
                        {
                            obj<ReceiveTCPKey>(objKey, new ReceiveTCPKey.Setting()
                            {
                                Socket = value,
                                Address = address,
                                Port = port,
                                WaitTCPConnections = _waitTCPConnections,
                                EventNameWorkForWaitTCPConnections = Event.SERVER_CLIENT_WORK,
                            });
                        }
                    }))
                    {
                        Logger.S_I.To(this, $"check tcp key call success");
                    }
                    else Logger.S_I.To(this, $"check tcp key don't call");
                }
            },
            Event.SERVER_CLIENT_WORK);
        }

        public static bool TryTCPKey(Socket client)
        {
            return true;
        }

        void TCPShield.IListen.EndInitialize()
        {
            Logger.S_I.To(this, "TCPShield end initilize");

            invoke_event(State.Change, Event.SYSTEM);
        }

        void IServer.Destroy()
        {
            destroy();
        }

        void IServer.AddClientData(ClientData data)
        {
            invoke_event(() =>
            {
                SystemInformation($"add client data Login:{data.Login} Password:{data.Password}");

                Logger.S_I.To(this, $"add client data Login:{data.Login}");

                _clientsData.Add(data.Login, data);
            },
            Event.SERVER_CLIENT_WORK);
        }

        /// <summary>
        /// Action<bool> call - при вызове запускается таймер. По окончию которого будет произведена
        /// проверка на то, было ли осеществлено tcp соединение.
        /// </summary>
        protected void CreatingWaitTCPConnection(string remoteIP, string key, Action<bool> call, IReturn<object> @return)
        {
            if (try_fly(() =>
            {
                foreach (TCPConnection value in _waitTCPConnections)
                {
                    if (value.Key == key)
                    {
                        Logger.S_E.To(this, $"Клиент {@return.GetKey()} c ключом {key} уже ожидает подключения.");

                        call(false);

                        destroy();

                        return;
                    }
                }

                Logger.S_I.To(this, $"Добавлен клиент {@return.GetKey()} ожидающий новое TCP подключение по ключу {key}");

                call(true);

                _waitTCPConnections.Add(new TCPConnection()
                {
                    IP = remoteIP,
                    Key = key,
                    @Return = @return
                });
            }))
                Logger.S_I.To(this, $"CreatingWaitTCPConnection call");
            else
                Logger.S_I.To(this, $"CreatingWaitTCPConnection don't call");
        }

        protected void TryGetClientData(string login, IReturn<bool, ClientData> @return)
        {
            ClientData result;
            {
                if (_clientsData.TryGetValue(login, out result))
                {
                    Logger.S_I.To(this, $"Get {login} data");

                    @return.To(true, result);
                }
                else
                {
                    Logger.S_I.To(this, $"{login} data is null");

                    @return.To(false, result);
                }
            }
        }

        void IServer.Running() => _isRunning = true;
        void IServer.Stopping() => _isRunning = false;

    }
}