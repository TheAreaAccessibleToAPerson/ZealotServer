namespace Zealot.server
{
    public class Setting
    {
        public string ClientName { init; get; }

        /// <summary>
        /// Хранит имя события которое будет работать со списками и коллекциями.
        /// </summary> <summary>
        public string EventName { init; get; }

        public string DBName { init; get; }
        public string CollectionName { init; get; }
    }
}