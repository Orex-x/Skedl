namespace TestNavigation;

public partial class StackPage3 : ContentPage
{
	public StackPage3()
	{
		InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        // После успешной авторизации
        var mainPage = new AppShellTab(); // Создаем новую главную страницу
        Application.Current.MainPage = mainPage; // Заменяем текущую главную страницу на новую

    }
}