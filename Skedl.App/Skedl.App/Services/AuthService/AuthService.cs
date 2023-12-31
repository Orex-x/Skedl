﻿using Microsoft.Maui.ApplicationModel.Communication;
using Newtonsoft.Json;
using Skedl.App.Models.Api;
using Skedl.App.Models.Reg;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.UserService;
using System.Text;

namespace Skedl.App.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IApiClient _client;
        private readonly IUserService _userService;
        public AuthService(IApiClient client, IUserService userService)
        {
            _client = client;
            _userService = userService;
        }

        public async Task<User> SignInAsync(string emailOrLogin, string password)
        {
            var contentJson = JsonConvert.SerializeObject(new UserDto() 
            { 
                Email = emailOrLogin, 
                Password = password
            });

            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("auth", "Auth/Login", body, false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UserTokenModel>(content);

                _client.SetBearerToken(model.Token);
                _client.SetUniversityUrl(model.User.University);

                await SecureStorage.Default.SetAsync("access_token", model.Token);
                await SecureStorage.Default.SetAsync("refresh_token", model.User.RefreshToken);

                var university = await SecureStorage.GetAsync("university");
                var group_id = await _userService.GetGroupId();

                if (model.User.GroupId == 0 && group_id != 0)
                {
                    model.User.GroupId = group_id;
                }

                if(string.IsNullOrEmpty(model.User.University) && !string.IsNullOrEmpty(university))
                {
                    model.User.University = university;
                    _client.SetUniversityUrl(university);
                }

                var userJson = JsonConvert.SerializeObject(model.User);

                var userBody = new StringContent(userJson, Encoding.UTF8, "application/json");

                var responeLoadUserDetails = await _client.PostAsync("api", "LoadUserDetails", userBody);
                if (responeLoadUserDetails.IsSuccessStatusCode)
                {
                    var contentLoadUserDetails = await responeLoadUserDetails.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(contentLoadUserDetails);
                    return user;
                }

                return model.User;
            }

            return null;
        }

        public async Task<HttpResponseMessage> SendCodeAsync(string email)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"to", email},
            };

            return await _client.GetAsync("auth", $"Auth/SendCode", false, queryParams: queryParams);
        }

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"to", email},
                {"code", code}
            };

            var response = await _client.GetAsync("auth", $"Auth/VerifyCode", false, queryParams: queryParams);
            return response.IsSuccessStatusCode;
        }

        public async Task<User> RegistrationAsync(RegModel regModel)
        {
            var contentJson = JsonConvert.SerializeObject(regModel);
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("auth", "Auth/Register", body, false);

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
            var response = await _client.GetAsync("auth", $"Home/IsAuthorized", false);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var body = new StringContent(content, Encoding.UTF8, "application/json");
                var model = JsonConvert.DeserializeObject<User>(content);
                _client.SetUniversityUrl(model.University);

                var responeLoadUserDetails = await _client.PostAsync("api", "LoadUserDetails", body);
                if (responeLoadUserDetails.IsSuccessStatusCode)
                {
                    var contentLoadUserDetails = await responeLoadUserDetails.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<User>(contentLoadUserDetails);
                    
                    user.Avatar = model.Avatar;
                    user.AvatarName = model.AvatarName;
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
            var response = await _client.PostAsync("auth", "Auth/RefreshToken", body, false);
            
            if (response.IsSuccessStatusCode)
            {
                var new_token = await response.Content.ReadAsStringAsync();
                await SecureStorage.Default.SetAsync("access_token", new_token);
                _client.SetBearerToken(new_token);
                return new_token;
            }

            return string.Empty;
        }

        public async Task<HttpResponseMessage> SendCodeForRecoverPassword(string emailOrLogin)
        {
            var queryParams = new Dictionary<string, object>
            {
                {"emailOrLogin", emailOrLogin}
            };

            var response = await _client.GetAsync("auth", $"Auth/SendCodeForRecoverPassword", false, queryParams: queryParams);
            return response;
        }


        public async Task<HttpResponseMessage> RecoverPassword(RecoverPasswordModel model)
        {   
            var contentJson = JsonConvert.SerializeObject(model);
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            return await _client.PostAsync("auth", "Auth/RecoverPassword", body, false);
        }
    }
}
