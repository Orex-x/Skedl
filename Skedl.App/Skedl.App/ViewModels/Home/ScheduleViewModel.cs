using CommunityToolkit.Mvvm.ComponentModel;
using Skedl.App.Services.DataService;
using Skedl.App.Services.UserService;

namespace Skedl.App.ViewModels.Home
{
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IDataService _dataService;

        [ObservableProperty]
        private string date;

        public ScheduleViewModel(IUserService userService, IDataService dataService) 
        {
            _userService = userService;
            _dataService = dataService;
            Init();
        }

        private async void Init()
        {
            var sw = await _dataService.GetScheduleWeek(DateTime.Today.Date, _userService.GetGroupId());
            if (sw == null) return;
            Date = sw.StartDate.ToShortDateString();
        }


        // SecureStorage.Default.RemoveAll();
    }
}
