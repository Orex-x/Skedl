using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Pages;
using Skedl.App.Pages.Home;
using Skedl.App.Pages.RecoverPasswordPages;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;

namespace Skedl.App.ViewModels
{
    public partial class AuthViewModel : ObservableObject
    {

        [ObservableProperty]
        private string loginOrEmail;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly EventProvider _eventProvider;
        public AuthViewModel(EventProvider eventProvider, IAuthService authService, IUserService userService)
        {
            _eventProvider = eventProvider;
            _authService = authService;
            _userService = userService;
        }

        [RelayCommand]
        async Task SignIn()
        {
            if(string.IsNullOrEmpty(LoginOrEmail) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Заполните поля";
                return;
            }

            var user = await _authService.SignInAsync(LoginOrEmail, Password);

            if(user == null)
            {
                ErrorMessage = "Не верные логин или пароль";
                return;
            }


            _userService.SaveUser(user);

            _eventProvider.CallUserDataReady();

            await Shell.Current.GoToAsync(nameof(AccountPage));
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
