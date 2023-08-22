﻿using Skedl.App.Pages;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services.ApiClient;
using Skedl.App.ViewModels;
using Skedl.App.ViewModels.RegViewModels;

namespace Skedl.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<IApiClient>(new ApiClient("http://192.168.0.117:8081"));

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<GroupsPage>();
        builder.Services.AddTransient<GroupsViewModel>();

        builder.Services.AddTransient<AuthPage>();
        builder.Services.AddTransient<AuthViewModel>();

        builder.Services.AddTransient<MailPage>();
        builder.Services.AddTransient<MailViewModel>();

        builder.Services.AddTransient<MailConfirmPage>();
        builder.Services.AddTransient<MailConfirmViewModel>();

        builder.Services.AddTransient<BioPage>();
        builder.Services.AddTransient<BioViewModel>();
        
        builder.Services.AddTransient<ChooseUniversityPage>();
        builder.Services.AddTransient<ChooseUniversityViewModel>();

        return builder.Build();
	}
}
