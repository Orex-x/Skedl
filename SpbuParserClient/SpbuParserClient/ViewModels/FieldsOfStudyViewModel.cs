using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpbuParserClient.Api;
using SpbuParserClient.Models;
using System.Collections.ObjectModel;

namespace SpbuParserClient.ViewModels
{

 
    public partial class FieldsOfStudyViewModel : ObservableObject, IQueryAttributable
    {
        private ApiClient _apiClient;

        [ObservableProperty]
        ObservableCollection<BaseLink> items;

        public FieldsOfStudyViewModel() 
        {
            Items = new ObservableCollection<BaseLink>() { 
                new BaseLink()
                {
                    Link = "asd",
                    Name = "asd"
                }
            };
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            _apiClient = query["ApiClient"] as ApiClient;
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
