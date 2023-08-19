using Newtonsoft.Json;
using Skedl.App.Models.Api;
using System.Net.Http.Headers;


namespace Skedl.App.Services.ApiClient
{
    public class ApiClient : IApiClient
    {
        private HttpClient _httpClient;
        private string _universityUrl;
        private string _token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoi0JTQsNC90Y8iLCJyb2xlIjoiVXNlciIsInR5cGUiOiJhY2Nlc3MiLCJleHAiOjE2OTI2ODYzNDJ9.mPsNEA_Gl90JhdOoi0cwMjRYH7JGp_PQqM8eSzZOdAmI6vHcaJFJeqC96OxRQdFcG5uA5wwDOw0EHKV5BwbcqQ";

        public ApiClient(string baseUrl)
        {

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

        }

        public void SetUniversityUrl(string url)
        {
            _universityUrl = url;
        }

        public async Task<ICollection<Group>> GetGroups()
        {
            try
            {
                var respone = await Get("/GetGroups");
                return JsonConvert.DeserializeObject<ICollection<Group>>(respone);
            }
            catch (Exception ex)
            {

            }
           return new List<Group>();
        }

        public async Task<string> Get(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_universityUrl}{endpoint}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Request failed with status code: {response.StatusCode}";
            }
        }
    }
}
