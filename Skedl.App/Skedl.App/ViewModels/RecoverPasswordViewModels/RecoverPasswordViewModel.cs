using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Pages;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;

namespace Skedl.App.ViewModels.RecoverPasswordViewModels
{
    [QueryProperty("EmailOrLogin", "EmailOrLogin")]
    [QueryProperty("OldPassword", "OldPassword")]
    public partial class RecoverPasswordViewModel : ObservableObject
    {

        [ObservableProperty]
        private string emailOrLogin;

        [ObservableProperty]
        private string oldPassword;
        
        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string rPassword;

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public RecoverPasswordViewModel(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [RelayCommand]
        async Task Next()
        {
            if (Password.Length < 6)
            {
                ErrorMessage = "Пароль должен быть более 5 символов";
                return;
            }

            if (Password != RPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return;
            }

            var result = await _authService.RecoverPassword(EmailOrLogin, OldPassword, Password);

            if (result.IsSuccessStatusCode)
            {
                await Shell.Current.GoToAsync(nameof(AuthPage));
            }
            else
            {
                ErrorMessage = await result.Content.ReadAsStringAsync();
            }
        }
    }
}
