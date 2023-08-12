namespace Skedl.DataStorage.Services;

public class ApiClient
{
    private HttpClient _httpClient;

    public ApiClient(string uri)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(uri);
    }

    public async Task<string> Get(string endpoint)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return $"Request failed with status code: {response.StatusCode}";
        }
    }
}