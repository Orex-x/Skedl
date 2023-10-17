namespace Skedl.DataCatcher.Services.HttpServices;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    
    public HttpService(string uri)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(uri);
    }
    
    public async Task<HttpResponseMessage> GetAsync(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        return await _httpClient.SendAsync(request);
    }
    
    public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = content;
        
        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }
        return await _httpClient.SendAsync(request);
    }
}