using Skedl.App.ViewModels.RecoverPasswordViewModels;

namespace Skedl.App.Pages.RecoverPasswordPages;

public partial class CodePage : ContentPage
{
	public CodePage(CodeViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}