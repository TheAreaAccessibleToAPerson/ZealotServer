using System.Net;
using System.Net.Sockets;
using Butterfly;

namespace Zealot.client.listen._SSLShield.content
{
    public class Setting
    {
        public string Address { set; get; }
        public uint Port { set; get; }

        /// <summary>
        /// Все входящие сообщения нужно перенаправить в данный метод.
        /// </summary> <summary>
        public Action<Socket> _clientsListen { set; get; }
    }

    public abstract class Controller : Butterfly.Controller.Board.LocalField<Setting>
    {
        public const string NAME = "Shield" + SSLShield.NAME;

        private TcpListener _server { set; get; } = null;

        protected bool IsRunning = false;

        protected IInput I_startListener;
        protected IInput I_stopListener;

        private Action<Socket> _clientsListen;

        protected void Listen()
        {
            if (IsRunning)
            {
                try
                {
                    if (_server.Pending())
                    {
                        Logger.S_I.To(this, "Accept tcp client");

                        Socket client = _server.AcceptSocket();

                        _clientsListen(client);
                    }
                }
                catch (SocketException ex)
                {
                    Logger.S_I.To(this, ex.ToString());

                    destroy();

                    return;
                }
            }
        }

        protected bool Setting()
        {
            Logger.S_I.To(this, "start setting ...");
            {
                if (Field._clientsListen != null)
                {
                    Logger.S_I.To(this, $"client receive message initialize.");

                    _clientsListen = Field._clientsListen;
                }
                else
                {
                    Logger.S_E.To(this, $"client receive message don't initialize(field value is null)");

                    destroy();

                    return false;
                }
            }
            Logger.S_I.To(this, "end setting.");

            return true;
        }

        protected bool Bind()
        {
            if (try_fly(() =>
            {
                try
                {
                    Logger.S_I.To(this, $"starting bind server Address:{Field.Address}:{Field.Port}");

                    Logger.S_I.To(this, $"binding ...");
                    SystemInformation($"binding ...");

                    _server = new TcpListener(IPAddress.Parse(Field.Address), (int)Field.Port);

                    Logger.S_I.To(this, "bind.");
                    SystemInformation("bind.");

                    _server.Start();
                }
                catch (Exception ex)
                {
                    Logger.S_W.To(this, ex.ToString());

                    destroy();
                }
            }))
            {
                Logger.S_I.To(this, $"Bind call");

                return true;
            }
            else
            {
                Logger.S_I.To(this, $"Bind don't call");

                return false;
            }
        }

        protected bool StopListener()
        {
            IsRunning = false;

            try
            {
                Logger.S_I.To(this, $"stop listener");
                _server.Stop();
                Logger.S_I.To(this, $"listener set null");
                _server = null;
            }
            catch (Exception ex)
            {
                Logger.S_W.To(this, ex.ToString());

                return false;
            }

            return true;
        }
    }
}