using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skedl.AuthService.Services;
using Skedl.AuthService.Services.FileService;
using Skedl.AuthService.Services.UserService;

namespace Skedl.AuthService.Controllers;

public class HomeController : Controller
{ 
    private readonly DatabaseContext _context;
    private readonly IUserService _userService;
    private readonly IFileService _fileService;

    public HomeController(DatabaseContext context, IUserService userService, IFileService fileService)
    {
        _context = context;
        _userService = userService;
        _fileService = fileService;
    }

    [Authorize] 
    public ActionResult IsAuthorized()
    {
        var user = _context.Users
            .FirstOrDefault(x => x.Email == _userService.GetMyEmail());

        if (user == null) return BadRequest();

        return Ok(user);
    }

    [HttpGet]
    public IActionResult Hello()
    {
        return Ok("Hello");
    }
}