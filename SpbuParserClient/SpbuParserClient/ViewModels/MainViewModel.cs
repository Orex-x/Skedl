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

        private ApiClientFactory _factory;

        public MainViewModel(ApiClientFactory factory)
        {

            _factory = factory;

            Items = new ObservableCollection<string>()
            {
                "СПБГУ",
                "МПТ",
            };
        }

        [RelayCommand]
        async void Tap(string item)
        {
            switch (item)
            {
                case "СПБГУ": {
                        _factory.BuildSpbuClient();
                        break;
                    }
                case "МПТ":
                    {
                        _factory.BuildMptClient();
                        break;
                    }
            }

            await Shell.Current.GoToAsync(nameof(FieldsOfStudyPage));
        }
    }
}
