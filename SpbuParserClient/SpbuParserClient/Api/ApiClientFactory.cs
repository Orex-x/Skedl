

namespace SpbuParserClient.Api
{
    public class ApiClientFactory
    {
        private ApiClient _client;

        private string _baseUri;

        public ApiClientFactory(string uri) 
        {
            _baseUri = uri;
        }

        public ApiClient GetApiClient() => _client;

        public ApiClientFactory SetBaseUri(string uri)
        {
            _baseUri = uri;
            return this;
        }

        public ApiClientFactory BuildSpbuClient()
        {
            _client = new SpbuApiClient(_baseUri);
            return this;
        }

        public ApiClientFactory BuildMptClient()
        {
            _client = new MptApiClient(_baseUri);
            return this;
        }
    }
}