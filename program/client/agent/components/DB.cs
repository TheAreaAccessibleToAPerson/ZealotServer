namespace Zealot.client.agent 
{
    public class DB 
    {
        public const string NAME = Agent.NAME + _.s + "DB"; 

        public struct Server 
        {
            public const string NAME = "Server" + DB.NAME;

            public struct Collection 
            {
                public const string NAME = "Server" + DB.NAME + ":Collection";
            }
        }
    }
}