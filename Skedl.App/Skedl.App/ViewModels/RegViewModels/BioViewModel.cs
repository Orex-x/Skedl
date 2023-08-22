using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Reg;
using Skedl.App.Services.ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private readonly IApiClient _apiClient;

        public BioViewModel(IApiClient apiClient)
        {
            _apiClient = apiClient;
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

            var resultOk = await _apiClient.Registration(Model);

            if(resultOk)
            {
                await Shell.Current.GoToAsync(nameof(MainPage));
            }
        }
    }
}
