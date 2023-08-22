using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skedl.AuthService.Services;

namespace Skedl.AuthService.Controllers;

public class HomeController : Controller
{
    private readonly DatabaseContext _context;

    public HomeController(DatabaseContext context)
    {
        _context = context;
    }
    
    public IActionResult Hello()
    {
        return Ok("Hello");
    }
}