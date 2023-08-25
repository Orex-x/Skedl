using Skedl.App.Pages;
using Skedl.App.Pages.Home;
using Skedl.App.Pages.RegPages;
using Skedl.App.Services.ApiClient;
using Skedl.App.Services.AuthService;
using Skedl.App.Services.DataService;
using Skedl.App.Services.UserService;
using Skedl.App.ViewModels;
using Skedl.App.ViewModels.Home;
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

        
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IDataService, DataService>();
        

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
        
        builder.Services.AddTransient<SchedulePage>();
        builder.Services.AddTransient<ScheduleViewModel>();

        return builder.Build();
	}
}
