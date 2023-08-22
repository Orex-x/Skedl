namespace TestNavigation;

public partial class TabPage2 : ContentPage
{
	public TabPage2()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        SecureStorage.Default.RemoveAll();
        // После успешной авторизации
        var mainPage = new AppShell(); // Создаем новую главную страницу
        Application.Current.MainPage = mainPage; // Заменяем текущую главную страницу на новую

    }
}