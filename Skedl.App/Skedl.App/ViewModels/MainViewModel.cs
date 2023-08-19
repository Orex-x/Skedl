using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.MainViewModel;
using Skedl.App.Pages;
using Skedl.App.Services.ApiClient;
using System.Collections.ObjectModel;

namespace Skedl.App.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<ListItem> items;

        private readonly IApiClient _apiClient;

        public MainViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;

            Items = new ObservableCollection<ListItem>()
            {
                new ListItem{ Name = "СПБГУ", Ling = "/Spbgu" },
                new ListItem{ Name = "МПТ", Ling = "/Mpu" },
            };
        }

        [RelayCommand]
        async void Tap(ListItem item)
        {
            _apiClient.SetUniversityUrl(item.Ling);
            await Shell.Current.GoToAsync(nameof(GroupsPage));
        }
    }
}
