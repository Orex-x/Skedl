using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpbuParserClient.Api;
using System.Collections.ObjectModel;

namespace SpbuParserClient.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<string> items;

        private readonly IApiClientFactory _apiClientFactory;

        public MainViewModel(IApiClientFactory apiClientFactory)
        {
            _apiClientFactory = apiClientFactory;
            Items = new ObservableCollection<string>()
            {
                "СПБГУ"
            };
        }



        [RelayCommand]
        async void Tap(string item)
        {

            ApiClient client = _apiClientFactory.CreateSpbuApiClient();

            await Shell.Current.GoToAsync(nameof(FieldsOfStudyPage), 
                new Dictionary<string, object>()
                {
                    { "ApiClient" , client }
                });
        }
    }
}
