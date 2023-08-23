using Newtonsoft.Json;
using Skedl.App.Models;
using Skedl.App.Models.Reg;
using Skedl.App.Services.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skedl.App.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _client;
        public AuthService(IApiClient client)
        {
            _client = client;
        }

        public Task Authorization(string emailOrLogin, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendCode(string email)
        {
            var response = await _client.Get("Auth", $"Auth/SendCode?to={email}", false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> VerifyCode(string email, string code)
        {
            var response = await _client.Get("Auth", $"Auth/VerifyCode?to={email}&code={code}", false);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Registration(RegModel model)
        {
            var contentJson = JsonConvert.SerializeObject(model);
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _client.Post("Auth", "Register", body, false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokensModel = JsonConvert.DeserializeObject<TokensModel>(content);

                await SecureStorage.Default.SetAsync("access_token", tokensModel.Token);
                await SecureStorage.Default.SetAsync("refresh_token", tokensModel.RefreshToken);
            }

            return response.IsSuccessStatusCode;
        }
    }
}
