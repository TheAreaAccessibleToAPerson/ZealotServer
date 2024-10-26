using System.Net.Sockets;

namespace Zealot.server
{
    public class TCPConnection
    {
        /// <summary>
        /// Данное соединение уставновлено, скоро клиент удалит эти данные из списка.
        /// </summary>
        public bool IsConnect = false;

        public string IP { set; get; }
        public string Key { set; get; }
        public Butterfly.IReturn<object> @Return { set;get; }
    }
}