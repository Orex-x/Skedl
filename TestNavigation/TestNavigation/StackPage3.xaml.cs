namespace TestNavigation;

public partial class StackPage3 : ContentPage
{
	public StackPage3()
	{
		InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        // ����� �������� �����������
        var mainPage = new AppShellTab(); // ������� ����� ������� ��������
        Application.Current.MainPage = mainPage; // �������� ������� ������� �������� �� �����

    }
}