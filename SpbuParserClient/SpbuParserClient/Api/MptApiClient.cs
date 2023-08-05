using Newtonsoft.Json;
using SpbuParserClient.Models;

namespace SpbuParserClient.Api
{
    public class MptApiClient : ApiClient
    {
        public MptApiClient(string uri) : base(uri)
        {
        }

        public override async Task<ICollection<FieldOfStudy>> GetFieldOfStudy(string code)
        {
            var json = await Get($"/api/getFieldOfStudy/{code}");
            return JsonConvert.DeserializeObject<ICollection<FieldOfStudy>>(json);
        }

        public override async Task<ICollection<BaseLink>> GetFieldsOfStudy()
        {
            var json = await Get("/api/getFieldsOfStudy");
            return JsonConvert.DeserializeObject<ICollection<BaseLink>>(json);
        }
    }
}
