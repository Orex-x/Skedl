using Newtonsoft.Json;
using Skedl.App.Models.Api;
using Skedl.App.Services.ApiClient;

namespace Skedl.App.Services.DataService
{
    public class DataService : IDataService
    {

        private readonly IApiClient _client;
        public DataService(IApiClient client)
        {
            _client = client;
        }

        public async Task<ICollection<Group>> GetGroups()
        {
            try
            {
                var response = await _client.Get("Api", "GetGroups");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ICollection<Group>>(result);
                }
            }
            catch (Exception ex) { }

            return new List<Group>();
        }
    }
}
