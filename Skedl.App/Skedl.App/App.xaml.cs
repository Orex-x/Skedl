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

    protected override async void OnStart()
    {
        base.OnStart();

        var access_token = await SecureStorage.GetAsync("access_token");
        var refresh_token = await SecureStorage.GetAsync("refresh_token");

        if(string.IsNullOrEmpty(access_token) || string.IsNullOrEmpty(refresh_token))
        {
            MainPage = new AppShellAuth();
            return;
        }

        _client.SetBearerToken(access_token);

        var stop = await NavigateBasedOnUserAuthorizationAsync();

        if (stop) return;

        var new_access_token = await _authService.RefreshTokenAsync(refresh_token);
        
        if (string.IsNullOrEmpty(new_access_token))
        {
            MainPage = new AppShellAuth();
            return;
        }

        stop = await NavigateBasedOnUserAuthorizationAsync();

        if (stop) return;

        MainPage = new AppShellHome();
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
