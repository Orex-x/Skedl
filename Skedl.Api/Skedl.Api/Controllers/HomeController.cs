using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skedl.Api.Models;
using Skedl.Api.Services.UserService;

namespace Skedl.Api.Controllers;

public class HomeController : Controller
{
    private readonly IUserService _userService;
    
    public HomeController(IUserService userService)
    {
        _userService = userService;
    }
    

    [HttpGet, Authorize]
    public ActionResult<string> GetMe()
    {
        var userName = _userService.GetMyName();
        return Ok(userName);
    }

  


    [HttpGet]
    public IActionResult Hello()
    {
        return Ok("Hello");
    }
}