using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services.ApiClient;

namespace Skedl.App.ViewModels.RegViewModels
{
    public partial class MailViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userEmail;

        private readonly IApiClient _apiClient;

        public MailViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }


        [RelayCommand]
        async Task Next()
        {
            var resultOk = await _apiClient.SendCode(UserEmail);

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
