using Skedl.App.Services;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;
using Skedl.App.ViewModels.Home;

namespace Skedl.App;

public partial class App : Application
{
    private readonly LoadDataService _loadDataService;
    public App(LoadDataService loadDataService)
	{
        /*SecureStorage.Default.Remove("university");
        SecureStorage.Default.Remove("group_id");*/

        _loadDataService = loadDataService;
        InitializeComponent();
        MainPage = new AppShell();
        _loadDataService.LoadUserData();
    }
}
