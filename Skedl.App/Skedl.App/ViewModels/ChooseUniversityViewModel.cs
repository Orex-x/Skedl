using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.ChooseUniversityViewModel;
using Skedl.App.Pages;
using Skedl.App.Services.ApiClient;
using System.Collections.ObjectModel;

namespace Skedl.App.ViewModels
{
    public partial class ChooseUniversityViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<ListItem> items;

        private readonly IApiClient _apiClient;

        public ChooseUniversityViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;

            Items = new ObservableCollection<ListItem>()
            {
                new ListItem{ Name = "СПБГУ", Ling = "Spbgu" },
                new ListItem{ Name = "МПТ", Ling = "Mpu" },
            };
        }

        [RelayCommand]
        async Task Tap(ListItem item)
        {
            _apiClient.SetUniversityUrl(item.Ling);
            await Shell.Current.GoToAsync(nameof(GroupsPage));
        }
    }
}
