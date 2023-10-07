﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Skedl.DataCatcher.Models.DB;
using Skedl.DataCatcher.Models.DTO;
using Skedl.DataCatcher.Services.DatabaseContexts;
using Skedl.DataCatcher.Services.HttpServices;
using Skedl.DataCatcher.Services.RabbitMqServices;
using System.Globalization;
using System.Text;

namespace Skedl.DataCatcher.Services.Spbgu
{
    public class SpbguScheduleCatch : ISpbguScheduleCatch
    {
        private readonly IHttpService _httpService;
        private readonly Dictionary<string, AsyncEventingBasicConsumer> _replyQueues;

        private readonly DatabaseSpbgu _db;

        private List<ScheduleLectureLocation> BufferScheduleLectureLocations;
        private List<ScheduleLectureSubject> BufferScheduleLectureSubjects;
        private List<ScheduleLectureTeacher> BufferScheduleLectureTeachers;
        private List<ScheduleLectureTime> BufferScheduleLectureTimes;

        public SpbguScheduleCatch(IRabbitMqService rabbitMqService, DatabaseSpbgu db, IHttpService httpService)
        {
            _replyQueues = new Dictionary<string, AsyncEventingBasicConsumer>();
            _httpService = httpService;
            _db = db;

            BufferScheduleLectureLocations = new();
            BufferScheduleLectureSubjects = new();
            BufferScheduleLectureTeachers = new();
            BufferScheduleLectureTimes = new();
        }

        public async Task<HttpResponseMessage> SendRequestAsync(string link)
        {
            string json = JsonConvert.SerializeObject(new { link = link });

            var body = new StringContent(json, Encoding.UTF8, "application/json");

            return await _httpService.PostAsync("/spbgu/getScheduleWeek", body);
        }

        public async Task CatchScheduleAsync(int countWeek = 1)
        {
            BufferScheduleLectureLocations = new();
            BufferScheduleLectureSubjects = new();
            BufferScheduleLectureTeachers = new();
            BufferScheduleLectureTimes = new();

            var groups = await _db.Groups.ToListAsync();

            foreach (var group in groups)
            {
                await FetchGroupDataForWeeksAsync(group, countWeek);
            }

            await _db.SaveChangesAsync();
        }

        public async Task FetchGroupDataForWeeksAsync(Group group, int countWeek)
        {
            string link = group.Link;

            for (int i = 0; i < countWeek; i++)
            {
                Console.WriteLine($"Group: {group.Name} | Link: {link}");
                
                var responseMessage = await SendRequestAsync(link);
                
                if (responseMessage.IsSuccessStatusCode)
                {
                    var nextLink = await ProcessAndSaveWeekScheduleAsync(responseMessage, group, countWeek);
                    
                    if (nextLink == null) break;

                    link = nextLink;

                    await _db.SaveChangesAsync();
                }
                else
                    Console.WriteLine($"{responseMessage.StatusCode} | {await responseMessage.Content.ReadAsStringAsync()}");
                
                await Task.Delay(2000);

            }
        }

        public async Task<string> ProcessAndSaveWeekScheduleAsync(HttpResponseMessage responseMessage, Group group, int countWeek)
        {
            var content = await responseMessage.Content.ReadAsStringAsync();

            var model = JsonConvert.DeserializeObject<ScheduleWeekDto>(content);

            if (model == null) return string.Empty;

            if (model.Days.Count == 0) return string.Empty;

            var dateStart = ParseDateTimeRu(model.Days.First().Date);

            DateTime monday = dateStart.AddDays(-(int)dateStart.DayOfWeek + (int)DayOfWeek.Monday);

            var scheduleWeekFinder = await _db.ScheduleWeeks.FindAsync(monday, group.Id);

            var list = await ImportScheduleDataAsync(model);

            if (scheduleWeekFinder == null)
            {
                var scheduleWeek = new ScheduleWeek()
                {
                    GroupId = group.Id,
                    StartDate = monday,
                    NextWeekLink = model.Next_Week_Link,
                    PreviousWeekLink = model.Previous_Week_Link,
                    Days = list
                };

                await _db.ScheduleWeeks.AddAsync(scheduleWeek);
            }
            else
            {
                scheduleWeekFinder.Days = list;
                _db.ScheduleWeeks.Update(scheduleWeekFinder);
            }

            return model.Next_Week_Link;
        }

        public async Task<List<ScheduleDay>> ImportScheduleDataAsync(ScheduleWeekDto model)
        {
            var list = new List<ScheduleDay>();

            foreach (var scheduleDayDto in model.Days)
            {
                var day = new ScheduleDay()
                {
                    Date = ParseDateTimeRu(scheduleDayDto.Date),
                    Lectures = new List<ScheduleLecture>()
                };

                foreach (var lecture in scheduleDayDto.Lectures)
                {
                    var location =
                        await _db.ScheduleLectureLocations.FirstOrDefaultAsync(x =>
                            x.Name == lecture.Location) ??
                        BufferScheduleLectureLocations.FirstOrDefault(x =>
                            x.Name == lecture.Location);

                    if (location == null)
                    {
                        location = new ScheduleLectureLocation { Name = lecture.Location };
                        BufferScheduleLectureLocations.Add(location);
                    }

                    var subject =
                        await _db.ScheduleLectureSubjects.FirstOrDefaultAsync(x =>
                            x.Name == lecture.Subject) ??
                        BufferScheduleLectureSubjects.FirstOrDefault(x =>
                            x.Name == lecture.Subject);

                    if (subject == null)
                    {
                        subject = new ScheduleLectureSubject { Name = lecture.Subject };
                        BufferScheduleLectureSubjects.Add(subject);
                    }

                    var teacher =
                        await _db.ScheduleLectureTeachers.FirstOrDefaultAsync(x =>
                            x.Name == lecture.Teacher) ??
                        BufferScheduleLectureTeachers.FirstOrDefault(x =>
                            x.Name == lecture.Teacher);

                    if (teacher == null)
                    {
                        teacher = new ScheduleLectureTeacher { Name = lecture.Teacher };
                        BufferScheduleLectureTeachers.Add(teacher);
                    }

                    var time =
                        await _db.ScheduleLectureTimes.FirstOrDefaultAsync(x =>
                            x.Name == lecture.Time) ??
                        BufferScheduleLectureTimes.FirstOrDefault(x =>
                            x.Name == lecture.Time);

                    if (time == null)
                    {
                        time = new ScheduleLectureTime { Name = lecture.Time };
                        BufferScheduleLectureTimes.Add(time);
                    }

                    var s = lecture.Status;

                    day.Lectures.Add(new ScheduleLecture
                    {
                        Location = location,
                        Subject = subject,
                        Teacher = teacher,
                        Time = time,
                        Status = s
                    });
                }

                list.Add(day);
            }

            return list;
        }

        private DateTime ParseDateTime(string dateString)
        {
            try
            {
                var provider = CultureInfo.InvariantCulture;
                return DateTime.Parse(dateString, provider);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }

            return DateTime.Now.AddDays(-10);
        }

        private DateTime ParseDateTimeRu(string dateString)
        {
            try
            {
                var provider = new CultureInfo("ru-RU"); // Устанавливаем русскую культуру
                string format = "dddd, d MMMM";
                return DateTime.ParseExact(dateString, format, provider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return DateTime.Now.AddDays(-10);
        }

    }
}