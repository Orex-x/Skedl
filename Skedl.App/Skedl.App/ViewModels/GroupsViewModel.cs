using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Api;
using Skedl.App.Pages.Home;
using Skedl.App.Services;
using Skedl.App.Services.DataService;
using Skedl.App.Services.UserService;
using System.Collections.ObjectModel;


namespace Skedl.App.ViewModels
{
    [QueryProperty("NavigateBack", "NavigateBack")]
    public partial class GroupsViewModel : ObservableObject
    {

        private ICollection<Group> _groups;

        [ObservableProperty]
        ObservableCollection<Group> items;

        [ObservableProperty]
        private string navigateBack;


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
        private readonly EventProvider _eventProvider;


        public GroupsViewModel(EventProvider eventProvider, IDataService dataService, IUserService userService)
        {
            _eventProvider = eventProvider;
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
            await _userService.SetGroup(group);
            await _userService.UpdateUserAsync();
            _eventProvider.CallUserDataReady();
            await Shell.Current.GoToAsync(NavigateBack ?? "..");
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
