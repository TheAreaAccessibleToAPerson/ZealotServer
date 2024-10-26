namespace Zealot.server
{
    public struct Data
    {
        public const string NAME = "server";

        public struct DB
        {
            public const string LOGIN_FIELD_NAME = "Login";
            public const string PASSWORD_FIELD_NAME = "Password";
        }
    }

    /// <summary>
    /// Здесь хранятся общие данные для всех клиентов.
    /// </summary> 
    public class ClientData
    {
        /// <summary>
        /// Логин клиента.
        /// </summary>
        public string Login { set; get; }

        /// <summary>
        /// Пароль клиента.
        /// </summary> 
        public string Password { set; get; }

        /// <summary>
        /// Подключен ли клиент уже к серверу.
        /// </summary> 
        public bool IsConnection { set; get; }

        /// <summary>
        /// Заблокирован ли данный пользователь.
        /// </summary> 
        public bool IsBlock { set; get; }
    }
}