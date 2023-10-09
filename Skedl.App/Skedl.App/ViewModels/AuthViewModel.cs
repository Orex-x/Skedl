using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Pages;
using Skedl.App.Pages.RecoverPasswordPages;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;
using Skedl.App.ViewModels.RecoverPasswordViewModels;

namespace Skedl.App.ViewModels
{
    public partial class AuthViewModel : ObservableObject
    {

        [ObservableProperty]
        private string loginOrEmail;

        [ObservableProperty]
        private string password;

        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        public AuthViewModel(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [RelayCommand]
        async Task SignIn()
        {
            if(string.IsNullOrEmpty(LoginOrEmail) || string.IsNullOrEmpty(Password))
            {
                return;
            }

            var user = await _authService.SignInAsync(LoginOrEmail, Password);

            if (user != null)
            {
                _userService.SaveUser(user);

                if(user.University == null)
                {
                    await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));
                    return;
                }

                if (user.Group == null)
                {
                    await Shell.Current.GoToAsync(nameof(GroupsPage));
                    return;
                }

                (Application.Current as App).SetShellHome();
            }
        }

        [RelayCommand]
        async Task Reg()
        {
            await Shell.Current.GoToAsync(nameof(MailPage));
        }

        [RelayCommand]
        async Task RecoverPassword()
        {
            await Shell.Current.GoToAsync(nameof(MailOrLoginPage));
        }
    }
}
