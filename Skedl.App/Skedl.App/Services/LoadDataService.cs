using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;
using System;

namespace Skedl.App.Services
{
    public class LoadDataService
    {
        private readonly IAuthService _authService;
        private readonly IApiClient _client;
        private readonly IUserService _userService;
        private readonly EventProvider _eventProvider;
       

        public LoadDataService(EventProvider eventProvider, IAuthService authService, IApiClient client, IUserService userService)
        {
            _eventProvider = eventProvider;
            _authService = authService;
            _client = client;
            _userService = userService;
        }

        public async void LoadUserData()
        {
            var access_token = SecureStorage.GetAsync("access_token").Result;
            var refresh_token = SecureStorage.GetAsync("refresh_token").Result;

            if (string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token))
            {
                return;
            }

            _client.SetBearerToken(access_token);

            var isAuthorized = await IsAuthorizedAsync();

            if (isAuthorized)
            {
                return;
            }

            var new_access_token = await _authService.RefreshTokenAsync(refresh_token);

            if (string.IsNullOrEmpty(new_access_token))
            {
                return;
            }

            await IsAuthorizedAsync();
        }

        public async Task<bool> IsAuthorizedAsync()
        {
            var user = await _authService.IsAuthorizedAsync();

            if (user != null)
            {
                _userService.SaveUser(user);
                _eventProvider.CallUserDataReady();
                return true;
            }

            return false;
        }
    }
}
