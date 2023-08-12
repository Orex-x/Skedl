using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skedl.DataStorage.Models;
using Skedl.DataStorage.Services;
using Skedl.DataStorage.Services.UserService;

namespace Skedl.DataStorage.Controllers;


public class DataController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;

    public DataController(DatabaseContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }
    
    [HttpGet, Authorize]
    public ActionResult<string> GetMe()
    {
        var userName = _userService.GetMyName();
        return Ok(userName);
    }

    public async Task<List<Group>> GetGroups() 
        => await _context.Groups.ToListAsync();
}