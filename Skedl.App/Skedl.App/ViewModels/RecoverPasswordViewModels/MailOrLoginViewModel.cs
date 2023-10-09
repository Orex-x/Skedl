using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Pages.RecoverPasswordPages;
using Skedl.App.Services.AuthService;

namespace Skedl.App.ViewModels.RecoverPasswordViewModels
{
    public partial class MailOrLoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string emailOrLogin;

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;

        public MailOrLoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }


        [RelayCommand]
        async Task Next()
        {
            var result = await _authService.SendCodeForRecoverPassword(EmailOrLogin);

            if (result.IsSuccessStatusCode)
            {
                var oldPassword = await result.Content.ReadAsStringAsync();
                await Shell.Current.GoToAsync($"{nameof(CodePage)}?EmailOrLogin={EmailOrLogin}&OldPassword={oldPassword}");
            }
            else
            {
                ErrorMessage = await result.Content.ReadAsStringAsync();
            }
        }
    }
}
