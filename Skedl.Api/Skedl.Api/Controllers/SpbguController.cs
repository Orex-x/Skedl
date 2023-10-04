﻿using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skedl.Api.Models;
using Skedl.Api.Services.Databases;
using Skedl.Api.Services.UserService;

namespace Skedl.Api.Controllers;

[Authorize]
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


    
    [Authorize]
    public async Task<IActionResult> GetScheduleWeek(string date, int groupId)
    {
        try
        {
            Console.WriteLine($"GetScheduleWeek date {date} | groupId {groupId}");

            if (DateTime.TryParseExact(date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var today))
            {
                Console.WriteLine("Parse true");
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
}