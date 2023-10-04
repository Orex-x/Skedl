﻿using Microsoft.EntityFrameworkCore;
using Skedl.DataCatcher.Models.DB;

namespace Skedl.DataCatcher.Services.DatabaseContexts;

public class DatabaseSpbgu : DbContext
{
    public DbSet<Group> Groups { get; set; }
    public DbSet<ScheduleDay> ScheduleDays { get; set; }
    public DbSet<ScheduleLecture> ScheduleLectures { get; set; }

    public DbSet<ScheduleLectureLocation> ScheduleLectureLocations { get; set; }
    public DbSet<ScheduleLectureSubject> ScheduleLectureSubjects { get; set; }
    public DbSet<ScheduleLectureTeacher> ScheduleLectureTeachers { get; set; }
    public DbSet<ScheduleLectureTime> ScheduleLectureTimes { get; set; }
    
    public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }

    public DatabaseSpbgu(string connectionString)
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