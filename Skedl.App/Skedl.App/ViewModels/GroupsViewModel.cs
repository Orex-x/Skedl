using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Api;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.DataService;
using Skedl.App.Services.UserService;
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
                    SearchAsync();
                }
            }
        }


        private readonly IDataService _dataService;
        private readonly IUserService _userService;

        public GroupsViewModel(IDataService dataService, IUserService userService)
        {
            _dataService = dataService;
            _userService = userService;
            Init();
        }

        public async void Init()
        {
            var list = await _dataService.GetGroupsAsync();
            _groups = list;
            Items = new(list);
        }

        [RelayCommand]
        async Task Tap(Group group)
        {
            _userService.SetGroup(group);
            await _userService.UpdateUserAsync();
            Application.Current.MainPage = new AppShellHome();
        }


        

        public async void SearchAsync()
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
