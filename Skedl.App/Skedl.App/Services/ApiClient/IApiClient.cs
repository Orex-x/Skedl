

using Skedl.App.Models.Api;
using Skedl.App.Models.Reg;

namespace Skedl.App.Services.ApiClient
{
    public interface IApiClient
    {
        void SetUniversityUrl(string url);

        Task<HttpResponseMessage> Post(string server, string endpoint, HttpContent content, bool withUniversity = true);

        Task<HttpResponseMessage> Get(string server, string endpoint, bool withUniversity = true);
    }
}
