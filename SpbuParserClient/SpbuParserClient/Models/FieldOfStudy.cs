namespace SpbuParserClient.Models
{
    public class FieldOfStudy
    {
        public string Name { get; set; }
        public ICollection<EduProgramme> EduProgrammes { get; set; }
    }
}
