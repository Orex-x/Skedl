

using Skedl.App.Models.Api;
using Skedl.App.Models.Reg;

namespace Skedl.App.Services.ApiClient
{
    public interface IApiClient
    {
        Task<ICollection<Group>> GetGroups();

        void SetUniversityUrl(string url);

        Task<bool> SendCode(string email);

        Task<bool> VerifyCode(string email, string code);

        Task<bool> Registration(RegModel model);
    }
}
