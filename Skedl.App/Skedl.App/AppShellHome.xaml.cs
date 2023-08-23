﻿using Skedl.App.Pages;
using Skedl.App.Pages.RegPages;

namespace Skedl.App;

public partial class AppShellHome : Shell
{
	public AppShellHome()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(BioPage), typeof(BioPage));
        Routing.RegisterRoute(nameof(MailConfirmPage), typeof(MailConfirmPage));
        Routing.RegisterRoute(nameof(MailPage), typeof(MailPage));
        Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
        Routing.RegisterRoute(nameof(ChooseUniversityPage), typeof(ChooseUniversityPage));
        Routing.RegisterRoute(nameof(GroupsPage), typeof(GroupsPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
    }
}