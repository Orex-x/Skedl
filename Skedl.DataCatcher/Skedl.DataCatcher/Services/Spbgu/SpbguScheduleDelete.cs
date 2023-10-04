using Quartz;
using Skedl.DataCatcher.Services.DatabaseContexts;

namespace Skedl.DataCatcher.Services.Spbgu
{
    public class SpbguScheduleDelete : ISpbguScheduleDelete
    {
        private readonly DatabaseSpbgu _db;

        public SpbguScheduleDelete(DatabaseSpbgu db)
        {
            _db = db;
        }

        public async Task DeleteLastWeek()
        {
            var day = DateTime.Now.AddDays(-7);
            DateTime monday = day.AddDays(-(int)day.DayOfWeek + (int)DayOfWeek.Monday);

            var scheduleWeek = _db.ScheduleWeeks.FirstOrDefault(x => x.StartDate == monday);
            if (scheduleWeek == null) return;

            _db.ScheduleWeeks.Remove(scheduleWeek);
            await _db.SaveChangesAsync();
        }
    }
}
