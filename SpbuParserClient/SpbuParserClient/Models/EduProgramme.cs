namespace SpbuParserClient.Models
{
    public class EduProgramme
    {
        public string Name { get; set; }

        public ICollection<BaseLink> Years { get; set; }
    }
}
