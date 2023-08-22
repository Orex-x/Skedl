using Newtonsoft.Json;
using Skedl.App.Models.Api;
using Skedl.App.Models.Reg;
using System.Net.Http.Headers;
using System.Text;

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
                var response = await Get("Api", "GetGroups");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ICollection<Group>>(result);
                } 
            }
            catch (Exception ex){}
            
            return new List<Group>();
        }

        public async Task<HttpResponseMessage> Get(string server, string endpoint, bool withUniversity = true)
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

        public async Task<HttpResponseMessage> Post(string server, string endpoint, HttpContent content, bool withUniversity = true)
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



        public async Task<bool> SendCode(string email)
        {
            var response = await Get("Auth", $"Auth/SendCode?to={email}", false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyCode(string email, string code)
        {
            var response = await Get("Auth", $"Auth/VerifyCode?to={email}&code={code}", false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Registration(RegModel model)
        {
            var contentJson = JsonConvert.SerializeObject(model);
            var content = new StringContent(contentJson, Encoding.UTF8, "application/json");

            var response = await Post("Auth", "Register", content, false);
            return response.IsSuccessStatusCode;
        }
    }
}
