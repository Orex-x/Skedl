using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;

namespace Skedl.App.ViewModels.RegViewModels
{
    public partial class MailViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userEmail;

        private readonly IAuthService _authService;

        public MailViewModel(IAuthService authService)
        {
            _authService = authService;
        }


        [RelayCommand]
        async Task Next()
        {
            var resultOk = await _authService.SendCode(UserEmail);

            if (resultOk)
            {
                var model = new RegModel
                {
                    Email = UserEmail
                };

                await Shell.Current.GoToAsync(nameof(MailConfirmPage), new Dictionary<string, object>()
                {
                    { "model", model }
                });
            }
        }
    }
}
