using Skedl.App.ViewModels;

namespace Skedl.App;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}

