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
            var sw = await _dataService.GetScheduleWeek(DateTime.Today.Date, _userService.GetGroupId());
            if (sw == null) return;

            foreach(var day in sw.Days) {

                days.Add(day.Date, day);

                string dayOfWeek = day.Date.ToString("ddd", new System.Globalization.CultureInfo("ru-RU"));
                string dayOfMonth = day.Date.Day.ToString();
                var brush = day.Date == DateTime.Today ? Brush.DarkBlue : Brush.Blue;
                
                if(day.Date == DateTime.Today)
                {
                    SelectedScheduleDay = day;
                    Date = day.Date.ToShortDateString();
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
            Date = button.DateTime.ToShortDateString();
            button.Background = Brush.DarkBlue;

            SelectedScheduleDay = days[button.DateTime];

            Buttons[currentSelectedButton].Background = Brush.Blue;
            currentSelectedButton = Buttons.IndexOf(button);
        }
    }
}
