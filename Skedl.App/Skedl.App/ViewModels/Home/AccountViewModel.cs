﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Api;
using Skedl.App.Pages;
using Skedl.App.Services.UserService;

namespace Skedl.App.ViewModels.Home
{
    public partial class AccountViewModel : ObservableObject
    {

        [ObservableProperty]
        private User user;


        public AccountViewModel(IUserService userService) 
        {
            User = userService.GetUser();
        }

        [RelayCommand]
        async Task SignOut()
        {
            SecureStorage.Default.RemoveAll();
            (Application.Current as App).SetShellAuth();
        }

        [RelayCommand]
        async Task ChangeUniversity()
        {
            await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));
        }

        [RelayCommand]
        async Task ChangeGroup()
        {
            await Shell.Current.GoToAsync(nameof(GroupsPage));
        }
    }
}
