using Skedl.App.ViewModels.Home;

namespace Skedl.App.Pages.Home;

public partial class AccountPage : ContentPage
{
	public AccountPage(AccountViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}