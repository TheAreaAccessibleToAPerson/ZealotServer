namespace Zealot.client.agent.ssl.read
{
    public class ClientAuthorization
    {
        public string Login { set; get; }
        public string Password { set; get; }
    }

    public class ResponseClientAuthorization
    {
        public enum Type
        {
            // Успешная авторизация.
            Success,
            // Невереный формат логина. 
            IvalidFormatLogin,
            // Неверный логин.
            InvalidLogin,
            // Невереный формат пароля.
            IvalidFormatPassword,
            // Неверный пароль.
            InvalidPassword,
            // Аккаунт заблокирован.
            Block,
        }

        /// <summary>
        /// Результат подключения.
        /// </summary> <summary>
        public Type Result { set; get; }

        /// <summary>
        /// Ключ для Tcp соединения.
        /// </summary> <summary>
        public string TcpKey {set;get;}
    }
}