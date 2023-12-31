﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skedl.App.Models.Api;
using Skedl.App.Pages;
using Skedl.App.Pages.Home;
using Skedl.App.Services;
using Skedl.App.Services.UserService;
using System;
using SkiaSharp;


namespace Skedl.App.ViewModels.Home
{
    public partial class AccountViewModel : ObservableObject
    {

        [ObservableProperty]
        private bool visibleAccount;

        [ObservableProperty]
        private bool visibleAuth;

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private ImageSource avatarSource;

        private readonly IUserService _userService;
        private readonly EventProvider _eventProvider;

        public INavigation Navigation { get; set; }

        public AccountViewModel(EventProvider eventProvider, IUserService userService) 
        {
            _eventProvider = eventProvider;
            _userService = userService;
            eventProvider.UserDataReady += LoadDataServiceUserDataReady;
            Task.Run(async () => await Init());
        }

        private async void LoadDataServiceUserDataReady(object sender, EventArgs e)
        {
            await Init();
        }

        public async Task Init()
        {
            await Task.Run(() =>
            {
                User = _userService.GetUser();

                if (User == null)
                {
                    VisibleAccount = false;
                    VisibleAuth = true;
                }
                else
                {
                    VisibleAccount = true;
                    VisibleAuth = false;

                    if (User.AvatarName != null)
                    {
                        AvatarSource = ImageSource.FromUri(new Uri(User.AvatarName));
                    }
                }
            });
        }

        [RelayCommand]
        async Task SignOut()
        {
            _userService.LogoutUser();
            _eventProvider.CallUserLogout();
            VisibleAccount = false;
            VisibleAuth = true;
        }

        [RelayCommand]
        async Task ChangeUniversity() => await Shell.Current.GoToAsync(nameof(ChooseUniversityPage));
        

        [RelayCommand]
        async Task ChangeGroup()
        {
            string nav = nameof(GroupsPage);

            if (string.IsNullOrEmpty(User.University))
                nav = nameof(ChooseUniversityPage);

            await Shell.Current.GoToAsync(nav,
                new Dictionary<string, object>() { { "NavigateBack", nameof(AccountPage) } });
        }
        
        
        [RelayCommand]
        async Task GoToAuth() => await Shell.Current.GoToAsync(nameof(AuthPage)); 
    }
}
