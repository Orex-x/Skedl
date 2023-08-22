using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Pages.RegPages;

namespace Skedl.App.ViewModels
{
    public partial class AuthViewModel : ObservableObject
    {

        [ObservableProperty]
        private string loginOrEmail;

        [ObservableProperty]
        private string password;

        public AuthViewModel()
        {
            
        }


        [RelayCommand]
        async Task Reg()
        {
            await Shell.Current.GoToAsync(nameof(MailPage));
        }
    }
}
