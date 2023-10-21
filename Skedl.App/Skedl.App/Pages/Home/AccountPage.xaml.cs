using Skedl.App.ViewModels.Home;

namespace Skedl.App.Pages.Home;

public partial class AccountPage : ContentPage
{
	public AccountPage(AccountViewModel vm)
	{
		InitializeComponent();
        vm.Navigation = Navigation;
        BindingContext = vm;
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }


}