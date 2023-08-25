using Skedl.App.Models.Api;

namespace Skedl.App.Services.UserService
{
    public interface IUserService
    {
        User GetUser();

        void SaveUser(User user);

        void SetUniversity(string  university);
        void SetGroup(Group group);

        Task<bool> UpdateUserAsync();
    }
}
