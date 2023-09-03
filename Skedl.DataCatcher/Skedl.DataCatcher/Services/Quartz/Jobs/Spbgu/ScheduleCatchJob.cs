﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using RabbitMQ.Client.Events;
using Skedl.DataCatcher.Models.DB;
using Skedl.DataCatcher.Models.DTO;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.RabbitMqServices;

namespace Skedl.DataCatcher.Services.Quartz.Spbgu;

public class ScheduleCatchJob : IJob
{
    private readonly IRabbitMqService _rabbitMqService;
    private readonly IHttpService _httpService;
    private readonly Dictionary<string, AsyncEventingBasicConsumer> _replyQueues;

    private readonly DatabaseSpbgu _db;
    
    public ScheduleCatchJob(IRabbitMqService rabbitMqService, DatabaseSpbgu db, IHttpService httpService)
    {
        _replyQueues = new Dictionary<string, AsyncEventingBasicConsumer>();
        _rabbitMqService = rabbitMqService;
        _httpService = httpService;
        _db = db;
    }

    
   
    public async Task Execute(IJobExecutionContext context)
    {
        var groups = await _db.Groups.ToListAsync();

        int i = 0;
        foreach (var group in groups)
        {
            Console.WriteLine($"Group: {group.Name} | Link: {group.Link}");

            try
            { 
                await Task.Delay(2000);
                
                string json = JsonConvert.SerializeObject(new { link = group.Link});
                
                var body = new StringContent(json, Encoding.UTF8, "application/json");

                var responseMessage = await _httpService.PostAsync("/spbgu/getScheduleWeek", body);

                if (responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("IsSuccessStatusCode");
                    var content = await responseMessage.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<ScheduleWeekDto>(content);

                    
                    if (model == null) continue;
                    
                    if(model.Days.Count == 0) continue;
                    
                    var dateStart = ParseDateTime(model.Days.First().Date);
                    
                    DateTime monday = dateStart.AddDays(-(int)dateStart.DayOfWeek + (int)DayOfWeek.Monday);

                    var sw = await _db.ScheduleWeeks.FindAsync(monday, group.Id);

                    if (sw == null)
                    {
                        var scheduleWeek = new ScheduleWeek()
                        {
                            NextWeekLink = model.Next_Week_Link,
                            PreviousWeekLink = model.Previous_Week_Link,
                            GroupId = group.Id,
                            StartDate = monday,
                            Days = new List<ScheduleDay>()
                        };

                        foreach (var scheduleDayDto in model.Days)
                        {
                            var day = new ScheduleDay()
                            {
                                Date = ParseDateTime(scheduleDayDto.Date),
                                Lectures = new List<ScheduleLecture>()
                            };

                            foreach (var lecture in scheduleDayDto.Lectures)
                            {
                                day.Lectures.Add(new ScheduleLecture()
                                {
                                    Location = lecture.Location,
                                    Subject = lecture.Subject,
                                    Teacher = lecture.Teacher,
                                    Time = lecture.Time
                                });
                            }
                            
                            scheduleWeek.Days.Add(day);
                        }
                        
                        await _db.ScheduleWeeks.AddAsync(scheduleWeek);
                    }
                    else
                    {
                        sw.NextWeekLink = model.Next_Week_Link;
                        sw.PreviousWeekLink = model.Previous_Week_Link;
                        sw.Days = new List<ScheduleDay>();
                      
                        foreach (var scheduleDayDto in model.Days)
                        {
                            var day = new ScheduleDay()
                            {
                                Date = ParseDateTime(scheduleDayDto.Date),
                                Lectures = new List<ScheduleLecture>()
                            };

                            foreach (var lecture in scheduleDayDto.Lectures)
                            {
                                day.Lectures.Add(new ScheduleLecture()
                                {
                                    Location = lecture.Location,
                                    Subject = lecture.Subject,
                                    Teacher = lecture.Teacher,
                                    Time = lecture.Time
                                });
                            }
                            
                            sw.Days.Add(day);
                        }
                        
                        _db.ScheduleWeeks.Update(sw);
                    }

                    i++;
                    if (i == 2)
                    {
                        Console.WriteLine("SaveChangesAsync");
                        await _db.SaveChangesAsync();
                        i = 0;
                    }
                }
                else
                {
                    Console.WriteLine($"{responseMessage.StatusCode} | {await responseMessage.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
            }
        }

        await _db.SaveChangesAsync();
    }


    private DateTime ParseDateTime(string dateString)
    {
        try
        {
            var provider = CultureInfo.InvariantCulture;
            return DateTime.Parse(dateString, provider);
        }
        catch (Exception ex){}

        return DateTime.Now.AddDays(-10);
    }
}