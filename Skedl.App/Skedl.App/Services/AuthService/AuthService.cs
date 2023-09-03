﻿using Newtonsoft.Json;
using Skedl.App.Models.Api;
using Skedl.App.Models.Reg;
using Skedl.App.Services.ApiClient;
using System.Text;

namespace Skedl.App.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _client;
        public AuthService(IApiClient client)
        {
            _client = client;
        }

        public async Task<User> SignInAsync(string emailOrLogin, string password)
        {
            var contentJson = JsonConvert.SerializeObject(new UserDto() 
            { 
                Email = emailOrLogin, 
                Password = password
            });


            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Auth", "Auth/Login", body, false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserTokenModel>(content);

                _client.SetBearerToken(model.Token);
                await SecureStorage.Default.SetAsync("access_token", model.Token);
                await SecureStorage.Default.SetAsync("refresh_token", model.User.RefreshToken);
                return model.User;
            }

            return null;
        }

        public async Task<HttpResponseMessage> SendCodeAsync(string email)
        {
            return await _client.GetAsync("Auth", $"Auth/SendCode?to={email}", false);
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var response = await _client.GetAsync("Auth", $"Auth/VerifyCode?to={email}&code={code}", false);
            return response.IsSuccessStatusCode;
        }

        public async Task<User> RegistrationAsync(RegModel regModel)
        {
            var contentJson = JsonConvert.SerializeObject(regModel);
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Auth", "Auth/Register", body, false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserTokenModel>(content);

                _client.SetBearerToken(model.Token);
                await SecureStorage.Default.SetAsync("access_token", model.Token);
                await SecureStorage.Default.SetAsync("refresh_token", model.User.RefreshToken);
                return model.User;

            }
            return null;
        }


      
        public async Task<User> IsAuthorizedAsync()
        {
            var response = await _client.GetAsync("Auth", $"Home/IsAuthorized", false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var body = new StringContent(content, Encoding.UTF8, "application/json");
                var model = JsonConvert.DeserializeObject<User>(content);
                _client.SetUniversityUrl(model.University);

                var responeLoadUserDetails = await _client.PostAsync("Api", "LoadUserDetails", body);
                if (responeLoadUserDetails.IsSuccessStatusCode)
                {
                    var contentLoadUserDetails = await responeLoadUserDetails.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(contentLoadUserDetails);
                    return user;
                }

                return model;
            }
            return null;
        }

        public async Task<string> RefreshTokenAsync(string token)
        {
            var contentJson = JsonConvert.SerializeObject(new RefreshTokenModel() { Token = token});
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("Auth", "Auth/RefreshToken", body, false);
            
            if (response.IsSuccessStatusCode)
            {
                var new_token = await response.Content.ReadAsStringAsync();
                _client.SetBearerToken(new_token);
                return new_token;
            }

            return string.Empty;
        }
    }
}
