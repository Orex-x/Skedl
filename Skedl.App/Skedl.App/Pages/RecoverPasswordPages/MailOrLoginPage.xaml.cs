using Skedl.App.ViewModels.RecoverPasswordViewModels;

namespace Skedl.App.Pages.RecoverPasswordPages;

public partial class MailOrLoginPage : ContentPage
{
	public MailOrLoginPage(MailOrLoginViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}