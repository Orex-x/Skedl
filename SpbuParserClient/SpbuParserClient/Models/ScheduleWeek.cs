namespace SpbuParserClient.Models
{
    public class ScheduleWeek
    {
        public string PrevionsWeekLink { get; set; }
        public string NextWeekLink { get; set; }
        public ICollection<ScheduleDay> Days { get; set; }
    }
}
