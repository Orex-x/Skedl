namespace Skedl.DataCatcher.Services.HttpServices;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;
    
    public HttpService(string uri)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(uri);
    }
    
    public async Task<string> Get(string endpoint, Dictionary<string, string>? headers = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
        }

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        
        return $"Request failed with status code: {response.StatusCode}";
    }
}