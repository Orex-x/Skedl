using Skedl.App.ViewModels.Home;

namespace Skedl.App.Pages.Home;

public partial class SchedulePage : ContentPage
{
	public SchedulePage(ScheduleViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}