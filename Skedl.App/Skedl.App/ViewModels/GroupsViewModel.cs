using CommunityToolkit.Mvvm.ComponentModel;
using Skedl.App.Models.Api;
using Skedl.App.Services.ApiClient;
using System.Collections.ObjectModel;


namespace Skedl.App.ViewModels
{
    public partial class GroupsViewModel : ObservableObject
    {

        private ICollection<Group> _groups;

        [ObservableProperty]
        ObservableCollection<Group> items;


        private string searchQuery;
   
        public string SearchQuery
        {
            get => searchQuery;
            set 
            {
                if (SetProperty(ref searchQuery, value))
                {
                    // Вызывайте вашу обработку изменений здесь, например:
                    SearchAsync();
                }
            }
        }


        private readonly IApiClient _apiClient;

        public GroupsViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
            Init();
        }

        
        public async void Init()
        {
            var list = await _apiClient.GetGroups();
            _groups = list;
            Items = new(list);
        }

        public async Task SearchAsync()
        {
            await Task.Run(() => {
                var list = _groups.Where(x => 
                    x.Name.ToLowerInvariant()
                    .Contains(SearchQuery.ToLowerInvariant()));

                Items = new(list);
            });
        }
    }
}
