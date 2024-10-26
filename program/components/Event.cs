namespace Zealot
{
    public struct Event
    {
        private const string NAME = "Event" + _.s;

        /// <summary>
        /// Системное событие.
        /// Изначально оно служит создание и уничтожение обьектов системы.
        /// Но ему можно делигировать выполнение и других задач.
        /// </summary> <summary>
        public const string SYSTEM = NAME + _.s + "System";

        /// <summary>
        /// Системное событие с отложеным вызовом минимум на 1 секунду.
        /// Изначально оно служит создание и уничтожение обьектов системы.
        /// Но ему можно делигировать выполнение и других задач.
        /// </summary> <summary>
        public const string SYSTEM_1000_TIME_STEP = SYSTEM + _.s + "1000 time step";

        /// <summary>
        /// Системное событие с отложеным вызовом минимум на 100 миллисекунд.
        /// Изначально оно служит создание и уничтожение обьектов системы.
        /// Но ему можно делигировать выполнение и других задач.
        /// </summary> <summary>
        public const string SYSTEM_100_TIME_STEP = SYSTEM + _.s + "100 time step";

        /// <summary>
        /// Системное событие с отложеным вызовом минимум на 5 секунду.
        /// Изначально оно служит создание и уничтожение обьектов системы.
        /// Но ему можно делигировать выполнение и других задач.
        /// </summary> <summary>
        public const string SYSTEM_5000_TIME_STEP = SYSTEM + _.s + "5000 time step";

        /// <summary>
        /// Событие обрабатывающее логер.
        /// </summary> <summary>
        public const string LOGGER = NAME + _.s + "Logger";

        /// <summary>
        /// Отвечает за работу сервера с клиентами.
        /// </summary> <summary>
        public const string SERVER_CLIENT_WORK = NAME + _.s + "ServerClientWork";

        /// <summary>
        /// Отвечает за работу сервера с клиентами.
        /// </summary> <summary>
        public const string SERVER_CLIENT_WORK_500_TIME_STEP = SERVER_CLIENT_WORK + _.s + "500 time step";

        /// <summary>
        /// Обрабатывает Receive
        /// </summary>
        public const string SERVER_CLIENTS_LISTEN = NAME + _.s + "ServerClientsListen";

        /// <summary>
        /// Прослушивает входящие сообщения.
        /// </summary>
        public const string SSL_RECEIVE = NAME + _.s + "SSLReceive";

        /// <summary>
        /// Отправляет исходящие сообщения.
        /// </summary>
        public const string SSL_SEND = NAME + _.s + "SSLSend";

        public const string TCP_SEND = NAME + _.s + "TCPSend";
        public const string TCP_RECEIVE = NAME + _.s + "TCPReceive";
    }
}