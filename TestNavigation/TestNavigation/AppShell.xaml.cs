namespace TestNavigation;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(StackPage1), typeof(StackPage1));
        Routing.RegisterRoute(nameof(StackPage2), typeof(StackPage2));
        Routing.RegisterRoute(nameof(StackPage3), typeof(StackPage3));
    }
}
