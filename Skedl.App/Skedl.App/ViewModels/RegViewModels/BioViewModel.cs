using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.UserService;

namespace Skedl.App.ViewModels.RegViewModels
{
    public partial class BioViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private RegModel model;

        [ObservableProperty]
        private string login;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string rPassword;

        [ObservableProperty]
        private string errorMessage;

        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public BioViewModel(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Model = query["model"] as RegModel;
        }

        [RelayCommand]
        async Task Next()
        {
            if(Login == String.Empty || Name == String.Empty)
            {
                ErrorMessage = "Заполните поля";
                return;
            }

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

            Model.Login = Login;
            Model.Name = Name;
            Model.Password = Password;

            var user = await _authService.RegistrationAsync(Model);

            if(user != null)
            {
                _userService.SaveUser(user);
                await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));
            }
        }
    }
}
