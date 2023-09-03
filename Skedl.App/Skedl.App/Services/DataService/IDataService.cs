﻿using Skedl.App.Models.Api;
namespace Skedl.App.Services.DataService
{
    public interface IDataService
    {
        Task<ICollection<Group>> GetGroupsAsync();
        Task<ScheduleWeek> GetScheduleWeek(DateTime date, int groupId);
    }
}
