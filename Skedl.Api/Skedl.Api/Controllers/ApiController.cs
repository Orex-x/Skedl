using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skedl.Api.Models;
using Skedl.Api.Services;
using Skedl.Api.Services.UserService;

namespace Skedl.Api.Controllers;

public class ApiController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;


    public ApiController(DatabaseContext context, IUserService userService)
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