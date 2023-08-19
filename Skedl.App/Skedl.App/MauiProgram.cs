using Skedl.App.Pages;
using Skedl.App.Services.ApiClient;
using Skedl.App.ViewModels;

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

        builder.Services.AddSingleton<IApiClient>(new ApiClient("http://localhost:5023"));

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddTransient<GroupsPage>();
        builder.Services.AddTransient<GroupsViewModel>();


        return builder.Build();
	}
}
