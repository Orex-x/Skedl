namespace Skedl.DataCatcher.Models.DTO;

public class ScheduleWeekDto
{
    public string Previous_Week_Link { get; set; }
    public string Next_Week_Link { get; set; }
    public ICollection<ScheduleDayDto> Days { get; set; } 
    
}