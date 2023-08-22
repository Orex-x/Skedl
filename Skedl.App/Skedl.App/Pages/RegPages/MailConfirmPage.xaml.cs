using Skedl.App.ViewModels.RegViewModels;

namespace Skedl.App.Pages.RegPages;

public partial class MailConfirmPage : ContentPage
{
	public MailConfirmPage(MailConfirmViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}