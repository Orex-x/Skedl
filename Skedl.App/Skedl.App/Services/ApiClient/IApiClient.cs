namespace Skedl.App.Services.ApiClient
{
    public interface IApiClient
    {
        void SetUniversityUrl(string url);

        void SetBearerToken(string token);

        Task<HttpResponseMessage> PostAsync(string server, string endpoint, HttpContent content, bool withUniversity = true);

        Task<HttpResponseMessage> GetAsync(string server, string endpoint, bool withUniversity = true, Dictionary<string, object> queryParams = null);
    }
}
