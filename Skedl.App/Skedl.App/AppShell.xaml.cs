using Skedl.App.Pages;

namespace Skedl.App;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(GroupsPage), typeof(GroupsPage));
    }
}
