using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skedl.AuthService.Services;
using Skedl.AuthService.Services.UserService;

namespace Skedl.AuthService.Controllers;

public class HomeController : Controller
{ 
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;
    public HomeController(DatabaseContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    [Authorize] 
    public ActionResult IsAuthorized()
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == _userService.GetMyEmail());
        
        return Ok(user);
    }
}