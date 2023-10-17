using Skedl.App.Models.Api;

namespace Skedl.App.Services.UserService
{
    public interface IUserService
    {
        User GetUser();
        void SaveUser(User user);
        Task SetUniversity(string university);
        Task SetGroup(Group group);
        Task<bool> UpdateUserAsync();
        Task<int> GetGroupId();
    }
}
