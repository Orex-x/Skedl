namespace TestNavigation;

public partial class AppShellTab : Shell
{
	public AppShellTab()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(TabPage1), typeof(TabPage1));
        Routing.RegisterRoute(nameof(TabPage2), typeof(TabPage2));
    }
}
