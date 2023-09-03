namespace Skedl.DataCatcher.Models.DB;

public class ScheduleDay
{
    public int Id { get; set; }
    
    public DateTime Date { get; set; }
    
    public ICollection<ScheduleLecture> Lectures { get; set; } 
}