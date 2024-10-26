using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Zealot.client.tcp.write;

namespace Zealot.server
{
    /// <summary>
    /// Прослушиваем Ключ который должен придти от клиента, при создании TCP соединения.
    /// </summary> <summary>
    public sealed class ReceiveTCPKey : Butterfly.Controller.Board.LocalField<ReceiveTCPKey.Setting>
    {
        private bool isRunning = false;

        void Construction()
        {
            Logger.S_I.To(this, "start construction ...");
            {
                add_event(Event.TCP_RECEIVE, 50, () =>
                {
                    if (isRunning)
                    {
                        try
                        {
                            int length = Field.Socket.Available;

                            if (length > 0)
                            {
                                byte[] buffer = new byte[length];

                                Field.Socket.Receive(buffer, length, SocketFlags.None);

                                if (length < 7)
                                {
                                    Logger.S_E.To(this, $"Недопустимая длина сообщения. length={length}");

                                    destroy();

                                    return;
                                }

                                int headerLength = buffer[0];
                                if (headerLength == 7)
                                {
                                    int type = buffer[1] << 8;
                                    type += buffer[2];

                                    int messageLength = buffer[3] << 24 ^ buffer[4] << 16 ^ buffer[5] << 8 ^ buffer[6];

                                    if (messageLength + 7 > length)
                                    {
                                        Logger.S_E.To(this, $"Размер заголовка:{7} + длина сообщения указаная в заголовке:{messageLength}, " +
                                            $" но поступило {buffer.Length}");

                                        destroy();

                                        return;
                                    }

                                    if (type == client.tcp.write.Type.KEY)
                                    {
                                        string str = Encoding.UTF8.GetString(buffer, 7, messageLength);
                                        Key j = JsonSerializer.Deserialize<Key>(str);

                                        if (j.Value == "")
                                        {
                                            Logger.S_E.To(this, $"Пришол пустой ключ.");

                                            destroy();

                                            return;
                                        }
                                        else
                                        {
                                            // Поступил ключ.
                                            string key = j.Value;

                                            Logger.S_I.To(this, $"По tcp соединению пришел ключ {j.Value}, проверим ожидается ли соединение с таким ключом");

                                            invoke_event(() =>
                                            {
                                                if (try_fly(() =>
                                                {
                                                    if (Field.WaitTCPConnections != null)
                                                    {
                                                        foreach (TCPConnection i in Field.WaitTCPConnections)
                                                        {
                                                            if (i.Key == key)
                                                            {
                                                                if (i.IP == Field.Address)
                                                                {
                                                                    Logger.S_I.To(this, $"{Field.Address} ожидает соединения." +
                                                                        $"Поступивший ключ {key} совподает с ожидаемым. Продолжим установление соединения ...");

                                                                    i.Return.To(Field.Socket);

                                                                    destroy();

                                                                    return;
                                                                }
                                                                else 
                                                                {
                                                                    Logger.S_I.To(this, $"Ключ {key} поступил от {Field.Address}, " + 
                                                                        $"но ожидалось что данный ключ будет получен от {i.IP}");

                                                                    return;
                                                                }
                                                            }
                                                            else 
                                                            {
                                                                Logger.S_I.To(this, $"Никто из клиентов не ожидает поступившего ключа {key} от {Field.Address}");

                                                                destroy();

                                                                return;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Logger.S_E.To(this, $"Вы не передали ссылку на список хранящий ключи для tcp соединения.");

                                                        destroy();

                                                        return;
                                                    }
                                                }))
                                                {
                                                    Logger.S_I.To(this, "Check key for tcp connection call.");
                                                }
                                                else Logger.S_I.To(this, "Check key for tcp connection don't call.");
                                            },
                                            Field.EventNameWorkForWaitTCPConnections);

                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Logger.S_E.To(this, $"Ожидаелось что придет сообщение типа KEY({client.tcp.write.Type.KEY})");

                                        destroy();

                                        return;
                                    }
                                }
                                else
                                {
                                    Logger.S_E.To(this, $"Поступило сообщение заголовком длина которого равна {headerLength}.");

                                    destroy();

                                    return;
                                }
                            }
                            else
                            {
                                int mill = current_timer();

                                Logger.S_I.To(this, $"Ожидаем сообщение  mill {mill}.");

                                if (mill > 2000)
                                {
                                    Logger.S_I.To(this, "Время ожидаения ключа по tcp соединению истекло.");

                                    destroy();

                                    return;
                                }
                            }
                        }
                        catch (SocketException socketException)
                        {
                            Logger.S_E.To(this, socketException.ToString());

                            destroy();

                            return;
                        }
                        catch (ObjectDisposedException objectDisposedException)
                        {
                            Logger.S_E.To(this, objectDisposedException.ToString());

                            destroy();

                            return;
                        }
                        catch (Exception ex)
                        {
                            Logger.S_E.To(this, ex.ToString());

                            destroy();

                            return;
                        }

                    }
                });
            }
            Logger.S_I.To(this, "end construction.");
        }

        void Start()
        {
            Logger.S_I.To(this, "starting ...");
            {
                isRunning = true;
                start_timer();
            }
            Logger.S_I.To(this, "start.");
        }
        void Destroyed()
        {
            {
                isRunning = false;
            }
            Logger.S_I.To(this, "destroyed.");
        }

        public class Setting
        {
            public Socket Socket { init; get; }
            public string Address { init; get; }
            public int Port { init; get; }

            public List<TCPConnection> WaitTCPConnections { init; get; }

            /// <summary>
            /// Событие для работы со списком WaitTCPConnection
            /// </summary>
            public string EventNameWorkForWaitTCPConnections { init; get; }
        }
    }
}