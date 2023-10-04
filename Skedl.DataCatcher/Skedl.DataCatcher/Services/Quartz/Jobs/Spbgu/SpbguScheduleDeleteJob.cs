using Quartz;
using Skedl.DataCatcher.Services.DatabaseContexts;

namespace Skedl.DataCatcher.Services.Quartz.Jobs.Spbgu
{
    public class SpbguScheduleDeleteJob : IJob
    {
        private readonly DatabaseSpbgu _db;

        public SpbguScheduleDeleteJob(DatabaseSpbgu db)
        {
            _db = db;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var day = DateTime.Now.AddDays(-8);
            DateTime monday = day.AddDays(-(int)day.DayOfWeek + (int)DayOfWeek.Monday);

            var scheduleWeek = _db.ScheduleWeeks.FirstOrDefault(x => x.StartDate == monday);
            if (scheduleWeek == null) return;

            _db.ScheduleWeeks.Remove(scheduleWeek);
            await _db.SaveChangesAsync();
        }
    }
}
