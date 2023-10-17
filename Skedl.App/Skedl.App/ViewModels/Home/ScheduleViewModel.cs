using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Api;
using Skedl.App.Models.HomeViewModels;
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

        public ScheduleViewModel(IUserService userService, IDataService dataService) 
        {
            _userService = userService;
            _dataService = dataService;
            Buttons = new ObservableCollection<ButtonViewModel>();
            days = new Dictionary<DateTime, ScheduleDay>();
            Init();
        }

        private async void Init()
        {
            var today = DateTime.Today.Date;

            var scheduleDays = await _dataService.GetSchedule(today, _userService.GetGroupId(), 2);
            if (scheduleDays == null) return;

            DateTime monday = today.AddDays(-(int)today.DayOfWeek + 1);

            foreach (var day in scheduleDays)
            {
                days.Add(day.Date, day);
            }

            for (int i = 0; i < 14; i++)
            {
                var currentDate = monday.AddDays(i);
                var day = days.GetValueOrDefault(currentDate);
                if(day == null)
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
