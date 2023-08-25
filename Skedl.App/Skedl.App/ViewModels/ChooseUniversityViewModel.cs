using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.ChooseUniversityViewModel;
using Skedl.App.Pages;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.UserService;
using System.Collections.ObjectModel;

namespace Skedl.App.ViewModels
{
    public partial class ChooseUniversityViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<ListItem> items;

        private readonly IApiClient _apiClient;
        private readonly IUserService _userService;

        public ChooseUniversityViewModel(IApiClient apiClient, IUserService userService)
        {
            _apiClient = apiClient;
            _userService = userService;

            Items = new ObservableCollection<ListItem>()
            {
                new ListItem{ Name = "СПБГУ", Link = "Spbgu" },
                new ListItem{ Name = "МПТ", Link = "Mpu" },
            };
        }

        [RelayCommand]
        async Task Tap(ListItem item)
        {
            _userService.SetUniversity(item.Link);
            await _userService.UpdateUserAsync();
            await Shell.Current.GoToAsync(nameof(GroupsPage));
        }
    }
}
