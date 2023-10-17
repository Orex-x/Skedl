using Skedl.App.Pages;
using Skedl.App.Pages.Home;
using Skedl.App.Pages.RecoverPasswordPages;
using Skedl.App.Pages.RegPages;

namespace Skedl.App;

public partial class AppShellAuth : Shell
{
	public AppShellAuth()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(BioPage), typeof(BioPage));
        Routing.RegisterRoute(nameof(MailConfirmPage), typeof(MailConfirmPage));
        Routing.RegisterRoute(nameof(MailPage), typeof(MailPage));
        Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
        Routing.RegisterRoute(nameof(ChooseUniversityPage), typeof(ChooseUniversityPage));
        Routing.RegisterRoute(nameof(GroupsPage), typeof(GroupsPage));
        Routing.RegisterRoute(nameof(MailOrLoginPage), typeof(MailOrLoginPage));
        Routing.RegisterRoute(nameof(CodePage), typeof(CodePage));
        Routing.RegisterRoute(nameof(RecoverPasswordPage), typeof(RecoverPasswordPage));
        Routing.RegisterRoute(nameof(SchedulePage), typeof(SchedulePage));
    }
}
