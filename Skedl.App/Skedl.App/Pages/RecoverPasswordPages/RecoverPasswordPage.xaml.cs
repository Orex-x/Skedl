using Skedl.App.ViewModels.RecoverPasswordViewModels;

namespace Skedl.App.Pages.RecoverPasswordPages;

public partial class RecoverPasswordPage : ContentPage
{
	public RecoverPasswordPage(RecoverPasswordViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}