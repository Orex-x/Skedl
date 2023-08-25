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

        public User GetUser() => _user;

        public void SaveUser(User user)
        {
            _user = user;

            if(_user.University != null)
            {
                _apiClient.SetUniversityUrl(user.University);
            }
        }

        public void SetGroup(Group group) => _user.Group = group;

        public void SetUniversity(string university)
        {
            _user.University = university;
            _apiClient.SetUniversityUrl(university);
        }

        public async Task<bool> UpdateUserAsync()
        {
            if (_user == null) return false;

            var contentJson = JsonConvert.SerializeObject(_user);
            var body = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var response = await _apiClient.PostAsync("Auth", "Auth/UpdateUser", body, false);
            return response.IsSuccessStatusCode;
        }
    }
}
