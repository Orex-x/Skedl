using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skedl.Api.Models;
using Skedl.Api.Services.Databases;

namespace Skedl.Api.Controllers;

[Authorize]
public class SpbguController : Controller
{
    private readonly DatabaseSpbgu _context;
    
    public SpbguController(DatabaseSpbgu context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Group>> GetGroups() => 
        await _context.Groups.ToListAsync();
}