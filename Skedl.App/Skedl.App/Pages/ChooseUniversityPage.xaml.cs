using Skedl.App.ViewModels;

namespace Skedl.App.Pages;

public partial class ChooseUniversityPage : ContentPage
{
	public ChooseUniversityPage(ChooseUniversityViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}