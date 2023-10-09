using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Pages.RecoverPasswordPages;
using Skedl.App.Services.AuthService;

namespace Skedl.App.ViewModels.RecoverPasswordViewModels
{
    [QueryProperty("EmailOrLogin", "EmailOrLogin")]
    [QueryProperty("OldPassword", "OldPassword")]
    public partial class CodeViewModel : ObservableObject
    {
        [ObservableProperty]
        private string oldPassword;

        [ObservableProperty]
        private string code; 
        
        [ObservableProperty]
        private string emailOrLogin;

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;

        public CodeViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        async Task Next()
        {
            var resultOk = await _authService.VerifyCodeAsync(EmailOrLogin, Code);

            if (resultOk)
            {
                await Shell.Current.GoToAsync($"{nameof(RecoverPasswordPage)}?EmailOrLogin={EmailOrLogin}&OldPassword={OldPassword}");
            }
            else
            {
                ErrorMessage = "Не верный код";
            }
        }
    }
}
