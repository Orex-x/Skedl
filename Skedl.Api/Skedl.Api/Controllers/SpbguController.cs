using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skedl.Api.Models;
using Skedl.Api.Services.Databases;
using Skedl.Api.Services.UserService;

namespace Skedl.Api.Controllers;

public class SpbguController : Controller
{
    private readonly DatabaseSpbgu _context;
    private readonly IUserService _userService;
    
    public SpbguController(DatabaseSpbgu context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    
    [Authorize]
    public async Task<User> LoadUserDetails([FromBody] User user)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(x => x.Id == user.GroupId);
        user.Group = group;
        return user;
    }

    public async Task<IEnumerable<Group>> GetGroups() => 
        await _context.Groups.ToListAsync();

    public async Task<IActionResult> GetScheduleWeek(string date, int groupId)
    {
        try
        {
            Console.WriteLine($"GetScheduleWeek date {date} | groupId {groupId}");

            if (DateTime.TryParseExact(date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var today))
            {
                DateTime monday = today.AddDays(-(int)today.DayOfWeek + 1);

                var obj = await _context.ScheduleWeeks
                    .Include(x => x.Days)
                    .ThenInclude(x => x.Lectures)
                    .ThenInclude(x => x.Location)
                    .Include(x => x.Days)
                    .ThenInclude(x => x.Lectures)
                    .ThenInclude(x => x.Subject)
                    .Include(x => x.Days)
                    .ThenInclude(x => x.Lectures)
                    .ThenInclude(x => x.Teacher)
                    .Include(x => x.Days)
                    .ThenInclude(x => x.Lectures)
                    .ThenInclude(x => x.Time)
                    .FirstOrDefaultAsync(x => x.StartDate == monday && x.GroupId == groupId);

                if (obj == null) return Ok(obj);

                Console.WriteLine($"Obhect: {obj.NextWeekLink} | {obj.PreviousWeekLink} | {obj.Days.Count}");

                return Ok(obj);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex);
        }

        return NoContent();
    }


    public async Task<IActionResult> GetSchedule(string date, int groupId, int weekCount = 1)
    {
        try
        {
            Console.WriteLine($"GetScheduleWeek date {date} | groupId {groupId}");

            if (DateTime.TryParseExact(date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var today))
            {
                var list = new List<ScheduleDay>();

                for(int i = 0; i < weekCount; i++)
                {
                    DateTime monday = today.AddDays(-(int)today.DayOfWeek + 1 + (i * 7));

                    var scheduleWeek = await _context.ScheduleWeeks
                        .Include(x => x.Days)
                        .ThenInclude(x => x.Lectures)
                        .ThenInclude(x => x.Location)
                        .Include(x => x.Days)
                        .ThenInclude(x => x.Lectures)
                        .ThenInclude(x => x.Subject)
                        .Include(x => x.Days)
                        .ThenInclude(x => x.Lectures)
                        .ThenInclude(x => x.Teacher)
                        .Include(x => x.Days)
                        .ThenInclude(x => x.Lectures)
                        .ThenInclude(x => x.Time)
                        .FirstOrDefaultAsync(x => x.StartDate == monday && x.GroupId == groupId);

                    if (scheduleWeek == null) continue;

                    Console.WriteLine($"Object: {scheduleWeek.NextWeekLink} | {scheduleWeek.PreviousWeekLink} | {scheduleWeek.Days.Count}");

                    list.AddRange(scheduleWeek.Days);
                }

                return Ok(list);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return BadRequest(ex);
        }

        return NoContent();
    }
}