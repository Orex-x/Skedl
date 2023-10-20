using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Api;
using Skedl.App.Models.HomeViewModels;
using Skedl.App.Pages;
using Skedl.App.Services;
using Skedl.App.Services.DataService;
using Skedl.App.Services.UserService;
using System.Collections.ObjectModel;

namespace Skedl.App.ViewModels.Home
{
    public partial class ScheduleViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IDataService _dataService;

        [ObservableProperty]
        private string date;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        ObservableCollection<ButtonViewModel> buttons;

        private Dictionary<DateTime, ScheduleDay> days;

        [ObservableProperty]
        private ScheduleDay selectedScheduleDay;

        private int currentSelectedButton = 0;

        [ObservableProperty]
        private int spacing = 0;

        [ObservableProperty]
        private bool visibilityBtnChooseUniversity = false;

        [ObservableProperty]
        private bool visibilitySchedule = true;

        private readonly EventProvider _eventProvider;


        public ScheduleViewModel(EventProvider eventProvider, IUserService userService, IDataService dataService)
        {
            _eventProvider = eventProvider;
            _userService = userService;
            _dataService = dataService;
            Buttons = new ObservableCollection<ButtonViewModel>();
            days = new Dictionary<DateTime, ScheduleDay>();
            _eventProvider.UserDataReady += EventUserDataReady;
            _eventProvider.UserLogout += EventUserLogout;
            LoadDataFromStorage();
        }

        private void EventUserLogout(object sender, EventArgs e)
        {
            days.Clear();
            Buttons.Clear();
            LoadDataFromStorage();
        }

        public async void LoadDataFromStorage()
        {
            VisibilityBtnChooseUniversity = false;
            VisibilitySchedule = true;

            var emptyStorage = true;

            var showSchedule = false;

            if (!emptyStorage)
            {
                showSchedule = true;
                //показывам ранее сохраненое расписание
            }

            var university = await SecureStorage.GetAsync("university");
            var groupId = await SecureStorage.GetAsync("group_id");

            if(!string.IsNullOrEmpty(groupId) && !string.IsNullOrEmpty(university))
            {
                showSchedule = true;
                var today = DateTime.Today.Date;
                var scheduleDays = await _dataService.GetSchedule(today, Convert.ToInt32(groupId), 2);
                UpdateSchedule(scheduleDays);
            }

            if (!showSchedule)
            {
                VisibilityBtnChooseUniversity = true;
                VisibilitySchedule = false;
            }
        }

        private async void EventUserDataReady(object sender, EventArgs e)
        {
            await UpdateSchedule();
        }

        public void UpdateSchedule(List<ScheduleDay> scheduleDays)
        {
            days.Clear();
            Buttons.Clear();

            if (scheduleDays == null) return;

            var today = DateTime.Today.Date;

            DateTime monday = today.AddDays(-(int)today.DayOfWeek + 1);

            foreach (var day in scheduleDays)
                days.Add(day.Date, day);
            
            for (int i = 0; i < 14; i++)
            {
                var currentDate = monday.AddDays(i);

                var day = days.GetValueOrDefault(currentDate);

                if (day == null)
                {
                    day = new ScheduleDay()
                    {
                        Date = currentDate,
                        Lectures = new List<ScheduleLecture>()
                    };
                    days.Add(currentDate, day);
                }

                string dayOfWeek = day.Date.ToString("ddd", new System.Globalization.CultureInfo("ru-RU"));
                string dayOfMonth = day.Date.Day.ToString();
                var brush = day.Date == DateTime.Today ? Brush.DarkBlue : Brush.Blue;

                if (day.Date == DateTime.Today)
                {
                    SelectedScheduleDay = day;
                    Date = day.Date.ToShortDateString();
                    if (SelectedScheduleDay.Lectures.Count == 0)
                    {
                        Message = "На сегодня отдых :3";
                    }
                    else
                        Message = "";
                }

                Buttons.Add(new ButtonViewModel()
                {
                    Text = dayOfWeek,
                    DayNumber = dayOfMonth,
                    Background = brush,
                    DateTime = day.Date
                });
            }
        }

        public async Task UpdateSchedule()
        {
            var groupId = await _userService.GetGroupId();

            if (groupId == 0) return;

            VisibilityBtnChooseUniversity = false;
            VisibilitySchedule = true;

            var today = DateTime.Today.Date;

            var scheduleDays = await _dataService.GetSchedule(today, groupId, 2);

            UpdateSchedule(scheduleDays);
        }

        [RelayCommand]
        async Task ChooseUniversity() => 
            await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));

        [RelayCommand]
        async Task Tap(ButtonViewModel button)
        {
            await Task.Run(() => {
                Date = button.DateTime.ToShortDateString();
                button.Background = Brush.DarkBlue;

                SelectedScheduleDay = days[button.DateTime];

                if (SelectedScheduleDay.Lectures.Count == 0)
                {
                    Message = "На сегодня отдых :3";
                }
                else
                    Message = "";

                Buttons[currentSelectedButton].Background = Brush.Blue;
                currentSelectedButton = Buttons.IndexOf(button);
            });
        }
    }
}
