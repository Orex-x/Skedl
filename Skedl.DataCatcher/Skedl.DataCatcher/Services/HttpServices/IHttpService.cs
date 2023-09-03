namespace Skedl.DataCatcher.Services.HttpServices;

public interface IHttpService
{
    Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, string>? headers = null);

    Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content,
        Dictionary<string, string>? headers = null);
}