using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpbuParserClient.Api;
using SpbuParserClient.Models;
using System.Collections.ObjectModel;

namespace SpbuParserClient.ViewModels
{

 
    public partial class FieldsOfStudyViewModel : ObservableObject
    {

        private ApiClient _client;

        [ObservableProperty]
        ObservableCollection<BaseLink> items;

        public FieldsOfStudyViewModel(ApiClientFactory factory) 
        {
            _client = factory.GetApiClient();
            Items = new ObservableCollection<BaseLink>();
            Init();
        }


        public async void Init()
        {
            Items = new(await _client.GetFieldsOfStudy());
        }
       

        [RelayCommand]
        async void Tap(BaseLink item)
        {
            await Shell.Current.GoToAsync(nameof(FieldOfStudyPage), new Dictionary<string, object>()
            {
                { "Field" , item }
            });
        }

  
    }
}
