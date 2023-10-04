using Microsoft.AspNetCore.Mvc;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.Quartz.Spbgu;

namespace Skedl.DataCatcher.Controllers
{
    public class SpbguController : Controller
    {
        private readonly QuartzService _quartz;
        public SpbguController(QuartzService quartz) 
        {
            _quartz = quartz;
        }

        public async Task<bool> ExecudeGroups()
        {
            var result = await _quartz.ExecuteJob(typeof(SpbguGroupCatchJob).Name);
            return result;
        }

        public async Task<bool> ExecudeSchedule()
        {
            var result = await _quartz.ExecuteJob(typeof(SpbguScheduleCatchJob).Name);
            return result;
        }
    }
}
