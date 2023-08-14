using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skedl.DataStorage.Services;
using Skedl.DataStorage.Services.Data;
using Skedl.DataStorage.Services.UserService;

namespace Skedl.DataStorage.Controllers;

public class ApiController : Controller
{
    private readonly IDataCatcher _dataCatcher;

    public ApiController(IDataCatcher dataCatcher)
    {
        _dataCatcher = dataCatcher;
    }
    
    [HttpGet, Authorize]
    public ActionResult UpdateDataForGroups()
    {
        _dataCatcher.UpdateGroups();
        return Ok();
    }
}