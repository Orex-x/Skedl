namespace SpbuParserClient;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(FieldOfStudyPage), typeof(FieldOfStudyPage));
		Routing.RegisterRoute(nameof(FieldsOfStudyPage), typeof(FieldsOfStudyPage));
		Routing.RegisterRoute(nameof(AuthPage), typeof(AuthPage));
	}
}
