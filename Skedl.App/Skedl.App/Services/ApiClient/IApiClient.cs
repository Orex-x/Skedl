

using Skedl.App.Models.Api;

namespace Skedl.App.Services.ApiClient
{
    public interface IApiClient
    {
        Task<ICollection<Group>> GetGroups();

        void SetUniversityUrl(string url);
    }
}
