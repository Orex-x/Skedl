using SpbuParserClient.ViewModels;

namespace SpbuParserClient;

public partial class FieldOfStudyPage : ContentPage
{
	public FieldOfStudyPage(FieldOfStudyViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}