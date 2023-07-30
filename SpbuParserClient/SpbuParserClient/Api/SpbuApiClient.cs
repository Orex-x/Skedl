using Newtonsoft.Json;
using SpbuParserClient.Models;

namespace SpbuParserClient.Api
{
    public class SpbuApiClient : ApiClient
    {

        public SpbuApiClient(string uri) : base(uri)
        {

        }

        public async override Task<ICollection<FieldOfStudy>> GetFieldOfStudy(string code)
        {
            var json = await Get($"/api/getFieldOfStudy/{code}");
            return JsonConvert.DeserializeObject<ICollection<FieldOfStudy>>(json);
        }

        public async override Task<ICollection<BaseLink>> GetFieldsOfStudy()
        {
            var json = await Get("/api/getFieldsOfStudy");
            return JsonConvert.DeserializeObject<ICollection<BaseLink>>(json);
        }
    }
}
