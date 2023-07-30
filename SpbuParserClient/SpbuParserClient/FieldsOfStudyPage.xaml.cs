using SpbuParserClient.ViewModels;

namespace SpbuParserClient;

public partial class FieldsOfStudyPage : ContentPage
{
	public FieldsOfStudyPage(FieldsOfStudyViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}