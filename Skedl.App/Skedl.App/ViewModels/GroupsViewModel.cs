using CommunityToolkit.Mvvm.ComponentModel;
using Skedl.App.Models.Api;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.DataService;
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


        private readonly IDataService _dataService;

        public GroupsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Init();
        }

        
        public async void Init()
        {
            var list = await _dataService.GetGroups();
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
