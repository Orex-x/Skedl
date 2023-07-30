using SpbuParserClient.Api;
using SpbuParserClient.ViewModels;

namespace SpbuParserClient;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}

