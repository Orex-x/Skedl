using Skedl.App.ViewModels;

namespace Skedl.App.Pages;

public partial class AuthPage : ContentPage
{
	public AuthPage(AuthViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}