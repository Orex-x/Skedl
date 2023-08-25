using System.Net.Http.Headers;

namespace Skedl.App.Services.ApiClient
{
    public class ApiClient : IApiClient
    {
        private HttpClient _httpClient;
        private string _universityUrl;

        public ApiClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void SetUniversityUrl(string url)
        {
            _universityUrl = url;
        }

        public async Task<HttpResponseMessage> GetAsync(string server, string endpoint, bool withUniversity = true)
        {
            var uri = string.Empty;
            
            if(withUniversity)
            {
                uri = $"/{server}/{_universityUrl}/{endpoint}";
            }
            else
            {
                uri = $"/{server}/{endpoint}";
            }

            return await _httpClient.GetAsync(uri);
        }

        public async Task<HttpResponseMessage> PostAsync(string server, string endpoint, HttpContent content, bool withUniversity = true)
        {
            var uri = string.Empty;

            if (withUniversity)
            {
                uri = $"/{server}/{_universityUrl}/{endpoint}";
            }
            else
            {
                uri = $"/{server}/{endpoint}";
            }

            return await _httpClient.PostAsync(uri, content);
        }
    }
}
