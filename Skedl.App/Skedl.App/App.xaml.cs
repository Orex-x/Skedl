using Skedl.App.Models.Api;
using Skedl.App.Pages;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;

namespace Skedl.App;

public partial class App : Application
{
    private readonly IAuthService _authService;
    private readonly IApiClient _client;
    private readonly IUserService _userService;
	public App(IAuthService authService, IApiClient client, IUserService userService)
	{
        _authService = authService;
        _client = client;
        _userService = userService;

        InitializeComponent();

		MainPage = new AppShellAuth();
	}

    public void SetShellAuth()
    {
        MainPage = new AppShellAuth();
    }

    public void SetShellHome()
    {
        MainPage = new AppShellHome();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        //SecureStorage.Default.RemoveAll();

        var access_token = await SecureStorage.GetAsync("access_token");
        var refresh_token = await SecureStorage.GetAsync("refresh_token");

        if (string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token))
        {
            await NavigateNotAuthUser();
            return;
        }

        _client.SetBearerToken(access_token);

        var stop = await NavigateBasedOnUserAuthorizationAsync();

        if (stop) return;

        var new_access_token = await _authService.RefreshTokenAsync(refresh_token);
        
        if (string.IsNullOrEmpty(new_access_token))
        {
            await NavigateNotAuthUser();
            return;
        }

        stop = await NavigateBasedOnUserAuthorizationAsync();

        if (stop) return;

        MainPage = new AppShellAuth();
    }

    public async Task NavigateNotAuthUser() 
    {
        var university = await SecureStorage.GetAsync("university");
        var group_id = await SecureStorage.GetAsync("group_id");

        MainPage = new AppShellAuth();

        if (string.IsNullOrEmpty(university))
        {
            await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));
            return;
        }

        _client.SetUniversityUrl(university);

        if (string.IsNullOrEmpty(group_id))
        {
            await Shell.Current.GoToAsync(nameof(GroupsPage));
            return;
        }
    }

    public async Task<bool> NavigateBasedOnUserAuthorizationAsync()
    {
        var user = await _authService.IsAuthorizedAsync();

        if (user != null)
        {
            _userService.SaveUser(user);

            if (user.University == null)
            {
                MainPage = new AppShellAuth();
                await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));
                return true;
            }

            if (user.Group == null)
            {
                MainPage = new AppShellAuth();
                await Shell.Current.GoToAsync(nameof(GroupsPage));
                return true;
            }

            MainPage = new AppShellHome();
            return true;
        }

        return false;
    }
}
