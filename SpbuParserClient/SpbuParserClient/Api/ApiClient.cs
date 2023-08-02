using SpbuParserClient.Models;


namespace SpbuParserClient.Api
{
    public abstract class ApiClient : IApiClient
    {
        private HttpClient _httpClient;

        public ApiClient(string uri)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(uri);
        }

        public async Task<string> Get(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Request failed with status code: {response.StatusCode}";
            }
        }

        public abstract Task<ICollection<BaseLink>> GetFieldsOfStudy();

        public abstract Task<ICollection<FieldOfStudy>> GetFieldOfStudy(string code);
    }
}
