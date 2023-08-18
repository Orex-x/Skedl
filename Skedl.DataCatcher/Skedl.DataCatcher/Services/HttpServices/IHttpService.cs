namespace Skedl.DataCatcher.Services.HttpServices;

public interface IHttpService
{
    Task<string> Get(string endpoint, Dictionary<string, string>? headers = null);
}