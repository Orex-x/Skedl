using SpbuParserClient.Models;

namespace SpbuParserClient.Api
{
    public interface IApiClient
    {
        public Task<ICollection<BaseLink>> GetFieldsOfStudy();

        public Task<ICollection<FieldOfStudy>> GetFieldOfStudy(string code);
    }
}
