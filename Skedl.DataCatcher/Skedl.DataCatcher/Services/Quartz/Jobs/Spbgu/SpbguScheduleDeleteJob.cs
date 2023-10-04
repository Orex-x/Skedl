using Quartz;
using Skedl.DataCatcher.Services.Spbgu;

namespace Skedl.DataCatcher.Services.Quartz.Jobs.Spbgu
{
    public class SpbguScheduleDeleteJob : IJob
    {
        private readonly ISpbguScheduleDelete _scheduleDelete;

        public SpbguScheduleDeleteJob(ISpbguScheduleDelete scheduleDelete)
        {
            _scheduleDelete = scheduleDelete;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _scheduleDelete.DeleteLastWeek();
        }
    }
}
