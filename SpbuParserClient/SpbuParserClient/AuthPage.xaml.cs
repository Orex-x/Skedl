using SpbuParserClient.ViewModels;

namespace SpbuParserClient;

public partial class AuthPage : ContentPage
{
	public AuthPage(AuthViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}