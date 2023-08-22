namespace TestNavigation;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        MainPage = new AppShell();
        Start();
    }

	public async void Start()
	{
        var token = await SecureStorage.Default.GetAsync("access_token");
		
        if(token == null)
		{
            MainPage = new AppShell();
        }
        else
		{
            MainPage = new AppShellTab();
        }
    }   
}
