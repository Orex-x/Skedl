using Microsoft.EntityFrameworkCore;
using Skedl.DataCatcher.Models.DB;

namespace Skedl.DataCatcher.Services.DatabaseContexts;

public class DatabaseSpbgu : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<ScheduleDay> ScheduleDays { get; set; }
    public DbSet<ScheduleLecture> ScheduleLectures { get; set; }
    public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }

    public DatabaseSpbgu()
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "host=localhost;port=5432;database=SkedlDataSpbgu;username=postgres;password=123");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ScheduleWeek>()
            .HasKey(x => new { x.StartDate, x.GroupId });
    }
}