namespace TestNavigation;

public partial class StackPage1 : ContentPage
{
	public StackPage1()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(StackPage2));
    }
}