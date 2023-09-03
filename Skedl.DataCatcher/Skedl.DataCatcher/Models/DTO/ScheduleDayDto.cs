namespace Skedl.DataCatcher.Models.DTO;

public class ScheduleDayDto
{
    public string Date { get; set; }
    public ICollection<ScheduleLectureDto> Lectures { get; set; } 
}