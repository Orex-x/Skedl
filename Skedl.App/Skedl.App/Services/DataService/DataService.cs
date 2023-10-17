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

        public async Task<ICollection<Group>> GetGroupsAsync()
        {
            try
            {
                var response = await _client.GetAsync("api", "GetGroups");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ICollection<Group>>(result);
                }
            }
            catch (Exception ex) { }

            return new List<Group>();
        }

        public async Task<ScheduleWeek> GetScheduleWeek(DateTime date, int groupId)
        {
            try
            {
                var queryParams = new Dictionary<string, object>
                {
                    {"date", date},
                    {"groupId", groupId}
                };


                var response = await _client.GetAsync("api", "GetScheduleWeek", queryParams: queryParams);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ScheduleWeek>(result);
                }
            }
            catch (Exception ex) { }

            return new ScheduleWeek();
        }


        public async Task<List<ScheduleDay>> GetSchedule(DateTime date, int groupId, int weekCount = 1)
        {
            try
            {
                var queryParams = new Dictionary<string, object>
                {
                    {"date", date},
                    {"groupId", groupId},
                    {"weekCount", weekCount},
                };


                var response = await _client.GetAsync("api", "GetSchedule", queryParams: queryParams);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ScheduleDay>>(result);
                }
            }
            catch (Exception ex) { }

            return new List<ScheduleDay>();
        }
    }
}
