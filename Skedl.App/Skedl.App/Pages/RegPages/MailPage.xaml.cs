using Skedl.App.ViewModels.RegViewModels;

namespace Skedl.App.Pages.RegPages;

public partial class MailPage : ContentPage
{
	public MailPage(MailViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}