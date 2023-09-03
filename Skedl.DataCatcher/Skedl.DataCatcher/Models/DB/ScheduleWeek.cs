namespace Skedl.DataCatcher.Models.DB;

public class ScheduleWeek
{
    public DateTime StartDate { get; set; }
    public int GroupId { get; set; }
    
    public string PreviousWeekLink { get; set; }
    public string NextWeekLink { get; set; }
    public ICollection<ScheduleDay> Days { get; set; } 
}