namespace TestNavigation;

public partial class StackPage2 : ContentPage
{
	public StackPage2()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await SecureStorage.Default.SetAsync("access_token", "123");
        await Shell.Current.GoToAsync(nameof(StackPage3));
    }
}