namespace FacebookCore.APIs
{
    public abstract class ApiBaseClass
    {
        protected readonly FacebookClient FacebookClient;

        protected ApiBaseClass(FacebookClient client)
        {
            FacebookClient = client;
        }
    }
}
