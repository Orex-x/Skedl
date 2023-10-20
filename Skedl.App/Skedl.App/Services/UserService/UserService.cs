using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using Skedl.App.Models.Api;
using Skedl.App.Services.ApiClient;
using System.Text;

namespace Skedl.App.Services.UserService
{
    public class UserService : IUserService
    {
        private User _user;

        private readonly IApiClient _apiClient;

        public UserService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }


        public User GetUser() => this._user;

        public void SaveUser(User user)
        {
            SecureStorage.Default.Remove("university");
            SecureStorage.Default.Remove("group_id");

            _user = user;

            if (_user.University != null)
            {
                _apiClient.SetUniversityUrl(user.University);
            }
        }

        public async Task SetGroup(Group group)
        {
            if(_user != null)
            {
                _user.Group = group;
                _user.GroupId = group.Id;
            }
            else
            {
                await SecureStorage.Default
                    .SetAsync("group_id", group.Id.ToString());
            }
        }

        public async Task SetUniversity(string university)
        {
            if (_user != null)
            {
                _user.University = university;
            }
            else
            {
                await SecureStorage.Default
                    .SetAsync("university", university);
            }

           
            _apiClient.SetUniversityUrl(university);
        }

        public async Task<bool> UpdateUserAsync()
        {
            if (_user == null) return false;

            var contentJson = JsonConvert.SerializeObject(_user);
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync("auth", "Auth/UpdateUser", body, false);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetGroupId()
        {
            if(_user == null) 
                return Convert.ToInt32(await SecureStorage.GetAsync("group_id"));
            return _user.GroupId ?? 0;
        }

        public void LogoutUser()
        {
            SecureStorage.Default.Remove("access_token");
            SecureStorage.Default.Remove("refresh_token");
            SecureStorage.Default.Remove("university");
            SecureStorage.Default.Remove("group_id");
            _user = null;
        }
    }
}
