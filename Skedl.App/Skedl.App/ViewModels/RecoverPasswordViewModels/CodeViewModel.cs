using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages.RecoverPasswordPages;
using Skedl.App.Services.AuthService;

namespace Skedl.App.ViewModels.RecoverPasswordViewModels
{
    public partial class CodeViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private RecoverPasswordModel model;

        [ObservableProperty]
        private string code; 

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;

        public CodeViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Model = query["model"] as RecoverPasswordModel;
        }

        [RelayCommand]
        async Task Next()
        {
            var resultOk = await _authService.VerifyCodeAsync(Model.EmailOrLogin, Code);

            if (resultOk)
            {
                await Shell.Current.GoToAsync($"{nameof(RecoverPasswordPage)}", new Dictionary<string, object>()
                {
                    { "model", Model }
                });
            }
            else
            {
                ErrorMessage = "Не верный код";
            }
        }
    }
}
