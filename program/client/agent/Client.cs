using Zealot.client.agent.ssl.read;

namespace Zealot.client
{
    public sealed class Agent : agent.Controller
    {
        void Construction()
        {
            Logger.S_I.To(this, "start construction ...");
            {
                Data = new(this);
                SSLWrite = new(this, Field.TcpClient);
                SSLRead = new(this, Field.TcpClient);
                TCPWrite = new(this);
                TCPRead = new(this);
                State = new(this);

                input_to(ref SSLWrite.I_send, Event.SSL_SEND, SSLWrite.Send);
                input_to(ref State.I_connection, () =>
                {
                    SSLWrite.Connection();

                    SystemInformation("CONNECTION");
                });

                add_event(Event.SSL_RECEIVE, SSLRead.Receive);

                // Принимаем данные для авторизации.
                input_to_1_1<ClientAuthorization, string>(ref SSLRead.I_authorization,
                    Event.SYSTEM, (value, @return) =>
                    {
                        if (State.Change()) Data.StartAuthorization(value, @return);
                    })
                    .send_echo_to<bool, server.ClientData>(Server<Agent>.BUS.Client.Echo.TRY_GET_DATA)
                        .output_to(Data.EndAuthorization);

                // Регистрирует прослушку подключения для tcp соединения от клиентa.
                input_to_0_3<string, string, Action<bool>>(ref Data.I_creatintTCPListener,
                    Event.SYSTEM, CreatingTCPListner)
                        .send_echo_to<object>(Server<Agent>.BUS.Client.Echo.WAIT_TCP_CONNECTION)
                            .output_to(EndTcpListener, Event.SYSTEM);

            }
            Logger.S_I.To(this, "end construction.");
        }

        void Start()
        {
            Logger.S_I.To(this, "starting ...");
            {
                State.Change();

                SSLRead.Start();
                SSLWrite.Start();
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
            }
            Logger.S_I.To(this, "end configurate.");
        }

        void Destroyed()
        {
            {
                SSLWrite.Disconnection();

                Stopping();
            }
            Logger.S_I.To(this, "destroyed.");
        }

        void Stop()
        {
            Logger.S_I.To(this, "stopping ...");
            {
                Stopping();
                Closing();
            }
            Logger.S_I.To(this, "stop");
        }
    }
}