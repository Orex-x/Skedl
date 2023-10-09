using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services.AuthService;

namespace Skedl.App.ViewModels.RegViewModels
{

    public partial class MailConfirmViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private RegModel model;

        [ObservableProperty]
        private string code;

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;

        public MailConfirmViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Model = query["model"] as RegModel;
        }

        [RelayCommand]
        async Task Next()
        {
            var resultOk = await _authService.VerifyCodeAsync(Model.Email, Code);

            if(resultOk)
            {
                await Shell.Current.GoToAsync(nameof(BioPage), new Dictionary<string, object>()
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
