﻿using SpbuParserClient.Api;
using SpbuParserClient.ViewModels;

namespace SpbuParserClient;

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

        builder.Services.AddSingleton<IApiClientFactory, ApiClientFactory>();
        builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<FieldOfStudyPage>();
        builder.Services.AddTransient<FieldOfStudyViewModel>();

        builder.Services.AddTransient<FieldsOfStudyPage>();
        builder.Services.AddTransient<FieldsOfStudyViewModel>();


        return builder.Build();
	}
}
