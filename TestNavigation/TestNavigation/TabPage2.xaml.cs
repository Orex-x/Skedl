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
        // ����� �������� �����������
        var mainPage = new AppShell(); // ������� ����� ������� ��������
        Application.Current.MainPage = mainPage; // �������� ������� ������� �������� �� �����

    }
}