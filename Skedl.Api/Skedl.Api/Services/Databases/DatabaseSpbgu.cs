using Microsoft.EntityFrameworkCore;
using Skedl.Api.Models;

namespace Skedl.Api.Services.Databases;

public class DatabaseSpbgu : DbContext
{

    public DbSet<Group> Groups { get; set; }
    public DbSet<ScheduleDay> ScheduleDays { get; set; }
    public DbSet<ScheduleLecture> ScheduleLectures { get; set; }
    public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }

    public DbSet<ScheduleLectureLocation> ScheduleLectureLocations { get; set; }
    public DbSet<ScheduleLectureSubject> ScheduleLectureSubjects { get; set; }
    public DbSet<ScheduleLectureTeacher> ScheduleLectureTeachers { get; set; }
    public DbSet<ScheduleLectureTime> ScheduleLectureTimes { get; set; }



    public DatabaseSpbgu(DbContextOptions<DatabaseSpbgu> options) : base(options)
    {
        try
        {
            Database.EnsureCreated();
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleWeek>()
            .HasKey(x => new { x.StartDate, x.GroupId });
    }
}