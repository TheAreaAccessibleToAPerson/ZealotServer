using Butterfly.system.objects.main;
using Zealot.client.listen;

namespace Zealot.server
{
    public interface IServer : IInformation, SSLShield.IListen, TCPShield.IListen
    {
        public void AddClientData(ClientData data);
        public void LoadingData();

        /// <summary>
        /// Сообщает о том что сервер запущен и готоров к подключению клиентов.
        /// </summary> 
        public void Running();

        /// <summary>
        /// Сообщает о том что сервер больше не будет принимать клиентов.
        /// </summary> 
        public void Stopping();

        public void Destroy();
    }
}