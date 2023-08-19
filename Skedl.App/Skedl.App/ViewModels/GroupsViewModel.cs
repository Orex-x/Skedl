using CommunityToolkit.Mvvm.ComponentModel;
using Skedl.App.Models.Api;
using Skedl.App.Services.ApiClient;
using System.Collections.ObjectModel;

namespace Skedl.App.ViewModels
{
    public partial class GroupsViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<Group> items;

        private readonly IApiClient _apiClient;

        public GroupsViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
            Init();
        }

        public async void Init()
        {
            var list = await _apiClient.GetGroups();
            Items = new(list);
        }
    }
}
