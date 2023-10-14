namespace Skedl.Api.Models;

public class ScheduleLecture
{
    public int Id { get; set; }
    public ScheduleLectureTime Time { get; set; }
    public ScheduleLectureSubject Subject { get; set; }
    public ScheduleLectureLocation Location { get; set; }
    public ScheduleLectureTeacher Teacher { get; set; }
    public ScheduleLectureStatus Status { get; set; }

}