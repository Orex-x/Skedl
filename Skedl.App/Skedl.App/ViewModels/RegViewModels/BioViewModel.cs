using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Pages;
using Skedl.App.Pages.Home;
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

        [ObservableProperty]
        private ImageSource avatarSource;

        [ObservableProperty]
        private int avatarPadding = 30;

        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public BioViewModel(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
            AvatarSource = "avatar.png";
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Model = query["passwordModel"] as RegModel;
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
                await Shell.Current.GoToAsync(nameof(AccountPage));
            }
        }


        [RelayCommand]
        async Task LoadAvatar()
        {
            PickOptions options = new()
            {
                PickerTitle = "Please select a image file",
                FileTypes = FilePickerFileType.Images,
            };

            var result = await PickAndShow(options);

            if (result != null)
            {
                using var fileStream = File.OpenRead(result.FullPath);

                byte[] bytes;

                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                Model.Avatar = bytes;
                Model.AvatarName = result.FileName;

                MemoryStream memory = new MemoryStream(bytes);
                AvatarSource = ImageSource.FromStream(() => memory);
                AvatarPadding = -40;
            }
        }
        
        public async Task<FileResult> PickAndShow(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                        result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = await result.OpenReadAsync();
                        var image = ImageSource.FromStream(() => stream);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // The user canceled or something went wrong
            }

            return null;
        }
    }
}
