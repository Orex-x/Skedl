using Microsoft.EntityFrameworkCore;
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
            var day = DateTime.Now.AddDays(-8);
            DateTime monday = day.AddDays(-(int)day.DayOfWeek + (int)DayOfWeek.Monday).Date;

            var scheduleWeek = _db.ScheduleWeeks
                .Include(x => x.Days)
                .ThenInclude(x => x.Lectures)
                .FirstOrDefault(x => x.StartDate == monday);

            if (scheduleWeek == null) return;

            _db.ScheduleWeeks.Remove(scheduleWeek);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteWeek(int days)
        {
            var day = DateTime.Now.AddDays(-days);
            DateTime monday = day.AddDays(-(int)day.DayOfWeek + (int)DayOfWeek.Monday).Date;

            var scheduleWeek = _db.ScheduleWeeks
                .Include(x => x.Days)
                .ThenInclude(x => x.Lectures)
                .FirstOrDefault(x => x.StartDate == monday);
            if (scheduleWeek == null) return;

            _db.ScheduleWeeks.Remove(scheduleWeek);
            
            await _db.SaveChangesAsync();
        }
    }
}
