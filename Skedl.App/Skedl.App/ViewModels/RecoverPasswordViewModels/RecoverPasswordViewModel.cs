using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages;
using Skedl.App.Pages.Home;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;

namespace Skedl.App.ViewModels.RecoverPasswordViewModels
{
    public partial class RecoverPasswordViewModel : ObservableObject, IQueryAttributable
    {

        [ObservableProperty]
        private RecoverPasswordModel model;

        [ObservableProperty]
        private string emailOrLogin;
        
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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Model = query["model"] as RecoverPasswordModel;
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

            Model.NewPassword = Password;

            var result = await _authService.RecoverPassword(Model);

            if (result.IsSuccessStatusCode)
            {
                await Shell.Current.GoToAsync(nameof(AccountPage));
            }
            else
            {
                ErrorMessage = await result.Content.ReadAsStringAsync();
            }
        }
    }
}
