using Butterfly;

namespace Zealot.client.listen._SSLShield
{
    public abstract class Controller : Butterfly.Controller
    {
        public const string NAME = "SSLListener";

        /// <summary>
        /// Задает место в которое будет перенаправлятся сообщения.
        /// </summary>
        private SSLShield.IListen _listen { set; get; } = null;

        private content.Setting _contentSetting;

        private string LocalAddress;
        private uint LocalPort;

        /// <summary>
        /// Данный метод запускает клиент для которого создается текущее tcp подключение 
        /// к серверу.
        /// </summary>
        protected void Initialize(SSLShield.IListen listen)
        {
            Logger.S_I.To(this, $"{listen.GetKey()} Передал интерфейс для получения входящих ssl подключений.");

            _listen = listen;

            _contentSetting._clientsListen = _listen.Listen;

            CreatingShield();
        }

        /// <summary>
        /// Соединение с сервером установлено.
        /// </summary> 
        protected void StartListener(IImpulsInformation info)
        {
            if (try_fly(() =>
            {
                Logger.S_I.To(this, "success start listen clients");

                SystemInformation("success start listen clients");

                if (_listen != null)
                {
                    Logger.S_I.To(this, $"Оповестим о начале прослушивания ssl подключений.");

                    _listen.EndInitialize();
                }
                else
                {
                    Logger.S_E.To(this, $"К моменту когда в оболочку придет подтверждение о начале прослушивания подключений поле _listen" +
                        "уже должно быть проинициализировано");

                    destroy();

                    return;
                }
            }))
            {
                Logger.S_I.To(this, "SuccessConnection call");
            }
            else Logger.S_I.To(this, "SuccessConnection don't call");
        }

        /// <summary>
        /// Ошибка соединения с сервером.
        /// </summary> 
        protected void StopListener(IImpulsInformation info)
        {
            if (try_fly(() => 
            {
                Logger.S_I.To(this, "stop listener clients");

                SystemInformation("stop listener clients");

                invoke_event(() => 
                {
                    try_fly(CreatingShield);
                }, 
                Event.SYSTEM_1000_TIME_STEP);
            }))
            {
                Logger.S_I.To(this, "StopListener call");
            }
            else Logger.S_I.To(this, "StopListner don't call");
        }

        void CreatingShield()
        {
            if (try_fly(() => { obj<Content>(Content.NAME, _contentSetting); }))
            {
                Logger.S_I.To(this, $"creating object {Content.NAME}");
            }
            else Logger.S_W.To(this, $"don't creating object {Content.NAME}");
        }

        protected bool TryGetServerAddress()
        {
            if (Data.TryGetServerSSLAddress(out string addr, out uint port, out string info))
            {
                Logger.S_I.To(this, info);

                SystemInformation(info);

                LocalAddress = addr;
                LocalPort = port;

                _contentSetting = new content.Setting()
                {
                    Address = addr,
                    Port = port
                };

                return true;
            }
            else
            {
                SystemInformation(info, ConsoleColor.Red);

                Logger.S_W.To(this, info);

                destroy();

                return false;
            }
        }

        private sealed class Content : content.Controller
        {
            void Construction()
            {
                Logger.S_I.To(this, "start construction ...");
                {
                    send_impuls(ref I_startListener, SSLShield.BUS.Content.Impuls.START_LISTENER);
                    send_impuls(ref I_stopListener, SSLShield.BUS.Content.Impuls.STOP_LISTENER);

                    add_event(Event.SERVER_CLIENTS_LISTEN, Listen);
                }
                Logger.S_I.To(this, "end construction.");
            }

            void Start()
            {
                SystemInformation("start");

                Logger.S_I.To(this, "starting ...");
                {
                    Logger.S_I.To(this, "send message: start listener clients.");

                    I_startListener.To();

                    IsRunning = true;
                }
                Logger.S_I.To(this, "start.");
            }

            void Destruction()
            {
                Logger.S_I.To(this, "start destruction ...");
                {
                    Logger.S_I.To(this, "send impuls: stop listen clients.");

                    if (StateInformation.IsCallConstruction)
                    {
                        I_stopListener.To();
                    }
                    else Logger.S_I.To(this, "don't call construction. don't send impuls: stop listen clients.");
                }
                Logger.S_I.To(this, "end destruction.");
            }

            void Configurate()
            {
                Logger.S_I.To(this, "start configurate ...");

                if (Setting())
                {
                    Logger.S_I.To(this, "setting success");

                    if (Bind())
                    {
                        Logger.S_I.To(this, "connection success");

                        Logger.S_I.To(this, "end configurate.");
                    }
                    else Logger.S_I.To(this, "configurate exception.");
                }
                else Logger.S_I.To(this, "configurate exception.");
            }

            void Destroyed()
            {
                Logger.S_I.To(this, "start destroyed ...");
                {
                    if (StopListener())
                    {
                        Logger.S_I.To(this, "stop listener success");
                    }
                }
                Logger.S_I.To(this, "end destroyed.");
            }

            void Stop()
            {
                Logger.S_I.To(this, "stopping ...");
                {
                }
                Logger.S_I.To(this, "stop");
            }
        }
    }
}