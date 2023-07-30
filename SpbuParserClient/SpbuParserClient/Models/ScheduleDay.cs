namespace SpbuParserClient.Models
{
    public class ScheduleDay
    {
        public string Date { get; set; }
        public ICollection<ScheduleLecture> Lectures { get; set; }
    }
}
