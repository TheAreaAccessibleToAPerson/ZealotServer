using System.Net.Sockets;
using System.Xml;
using Butterfly;

namespace Zealot.client.agent
{
    public class Controller : Butterfly.Controller.Board.LocalField<Setting>, IClient, Data.IClient
    {
        protected Data Data { set; get; }
        protected State State { set; get; }
        protected ssl.Write SSLWrite { set; get; }
        protected ssl.Read SSLRead { set; get; }
        protected tcp.Write TCPWrite { set; get; }
        protected tcp.Read TCPRead { set; get; }

        public const string NAME = "Agent";

        void IClient.LoadingData()
        {
            if (try_fly(() =>
            {
                Logger.S_I.To(this, "loading data ...");
            }))
            {
                Logger.S_I.To(this, "LoadingData call");

            }
            else Logger.S_I.To(this, "LoadingData don't call");
        }

        /// <summary>
        /// Отправляем данные для ожидания tcp подключения.
        /// </summary>
        protected void CreatingTCPListner(IReturn<string, string, Action<bool>> @return)
        {
            if (try_fly(() =>
            {
                if (SSLWrite.TryGetRemoveAddress(out string remoteAddress))
                {
                    Logger.S_I.To(this, $"creating tcp listener. RemoveAddress:{remoteAddress}, Key:{GetID()}");

                    @return.To(remoteAddress, GetID().ToString(), (bool result) =>
                    {
                        if (result)
                        {
                            Logger.S_I.To(this, $"Данные для поключения tcp соединения были добавлены на Server(RemoteAddress:{remoteAddress}, Key:{GetID()})" +
                                $". Отправим запрос клиенту");

                            invoke_event(() =>
                            {
                                if (try_fly(State.Change))
                                {
                                    Logger.S_I.To(this, $"State.Change call");

                                    invoke_event(() =>
                                    {
                                        if (try_fly(SSLWrite.RequestTCPConnection) && try_fly(State.Change))
                                        {
                                            Logger.S_I.To(this, $"SSLWrite.RequestTcpConnection call");

                                            invoke_event(() =>
                                            {
                                                if (try_fly(() =>
                                                {
                                                    // Если от клиента так и непоступило tcp соединение, 
                                                    // то закончим работу с ним.
                                                    if (State.CurrentState == State.Type.RequestTCPConnection)
                                                    {
                                                        Logger.S_W.To(this, $"Проверка на установление tcp соединение:неудалось установить tcp соединение.");

                                                        destroy();

                                                        return;
                                                    }
                                                    else Logger.S_I.To(this, $"Проверка на установление tcp соединение:tcp соединение установлено.");
                                                }))
                                                    Logger.S_I.To(this, $"CreatingTCPListener->checkTcpConnection call");
                                                else
                                                    Logger.S_I.To(this, $"CreatingTCPListener->checkTcpConnection don't call");
                                            },
                                            Event.SYSTEM_5000_TIME_STEP);
                                        }
                                        else Logger.S_I.To(this, $"SSLWrite.RequestTcpConnection don't call");
                                    },
                                    Event.SSL_SEND);
                                }
                                else Logger.S_I.To(this, $"State.Change don't call");
                            },
                            Event.SYSTEM);

                        }
                        else Logger.S_E.To(this, $"Неудалось добавить данные на Server, ожидается окончание работы программы.");
                    });
                }
                else
                {
                    Logger.S_E.To(this, $"Неудалось получить адрес клиента, поэтому невозможно создать прослушку нового tcp подключения.");

                    destroy();

                    return;
                }
            }))
            {
                Logger.S_I.To(this, "CreatingTCPListener call");
            }
            else Logger.S_I.To(this, "CreatingTCPListener don't call");
        }

        protected void EndTcpListener(object socket)
        {
            if (try_fly(() =>
            {
                Logger.S_I.To(this, "creating tcp listener ...");

                invoke_event(() =>
                {
                    TCPWrite.SetSocket(socket);
                    TCPRead.SetSocket(socket);

                    State.Change();
                },
                Event.SYSTEM);
            }))
            {
                Logger.S_I.To(this, "CreatingTCPListener call");
            }
            else Logger.S_I.To(this, "CreatingTCPListener don't call");
        }
    }
}