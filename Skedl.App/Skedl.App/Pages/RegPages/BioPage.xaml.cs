using Skedl.App.ViewModels.RegViewModels;

namespace Skedl.App.Pages.RegPages;

public partial class BioPage : ContentPage
{
	public BioPage(BioViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}