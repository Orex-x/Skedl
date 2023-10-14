using Microsoft.AspNetCore.Mvc;
using Skedl.DataCatcher.Services.Quartz;
using Skedl.DataCatcher.Services.Quartz.Spbgu;
using Skedl.DataCatcher.Services.Spbgu;

namespace Skedl.DataCatcher.Controllers
{
    public class SpbguController : Controller
    {
        private readonly ISpbguGroupCatch _spbguGroupCatch;
        private readonly ISpbguScheduleDelete _spbguScheduleDelete;
        private readonly ISpbguScheduleCatch _spbguScheduleCatch;
        public SpbguController(
            ISpbguGroupCatch spbguGroupCatch, 
            ISpbguScheduleDelete spbguScheduleDelete,
            ISpbguScheduleCatch spbguScheduleCatch)
        {
            spbguGroupCatch = spbguGroupCatch;
            _spbguScheduleDelete = spbguScheduleDelete;
            _spbguScheduleCatch = spbguScheduleCatch;
        }

        public async Task<bool> ExecudeGroups()
        {
            try
            {
                await _spbguGroupCatch.CatchGroups();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> ExecudeSchedule(int countWeek = 1)
        {
            try
            {
                await _spbguScheduleCatch.CatchScheduleAsync(countWeek);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public async Task<bool> ExecudeScheduleByGroup(string groupName, int countWeek = 1)
        {
            try
            {
                await _spbguScheduleCatch.CatchScheduleAsyncByGroup(countWeek, groupName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}
