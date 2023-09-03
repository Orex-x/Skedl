using System.Globalization;
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

    /*[Authorize]
    public async Task<ScheduleWeek?> GetScheduleWeek(int groupId)
    {
        DateTime today = DateTime.Today;
        DateTime monday = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);
        
        var obj = await _context.ScheduleWeeks
            .Include(x => x.Days)
            .ThenInclude(x => x.Lectures)
            .FirstOrDefaultAsync(x => x.StartDate == monday && x.GroupId == groupId);
        return obj;
    }*/
    
    [Authorize]
    public async Task<ScheduleWeek?> GetScheduleWeek(string date, int groupId)
    {
        if (DateTime.TryParseExact(date, "dd.MM.yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var today))
        {
            DateTime monday = today.AddDays(-(int)today.DayOfWeek + 1);
        
            var obj = await _context.ScheduleWeeks
                .Include(x => x.Days)
                .ThenInclude(x => x.Lectures)
                .FirstOrDefaultAsync(x => x.StartDate == monday && x.GroupId == groupId);
            return obj;
        }
        
        return null;
    }
    
}