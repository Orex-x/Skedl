namespace SpbuParserClient.Api
{
    public class ApiClientFactory : IApiClientFactory
    {
        public ApiClient CreateSpbuApiClient()
        {
            return new SpbuApiClient("http://192.168.0.126:8000");
        }
    }
}
