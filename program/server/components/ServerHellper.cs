using System.Net;
using System.Net.Sockets;

namespace Zealot.server
{
    public static class ServerHellper
    {
        public static bool GetAddressAndPort(Socket client, out string address, out int port, out string info)
        {
            try
            {
                address = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
            }
            catch (Exception ex)
            {
                address = "";
                port = 0;

                info = $"Недалось получить адрес из TcpClinet. - {ex}";

                return false;
            }

            try
            {
                port = ((IPEndPoint)client.RemoteEndPoint).Port;

                info = $"Address:{address} Port:{port}";

                return true;
            }
            catch (Exception ex)
            {
                address = "";
                port = 0;

                info = $"Недалось получить порт из TcpClinet. - {ex}";

                return false;
            }
        }
    }
}