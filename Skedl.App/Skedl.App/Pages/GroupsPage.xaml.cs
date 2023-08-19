using Skedl.App.ViewModels;

namespace Skedl.App.Pages;

public partial class GroupsPage : ContentPage
{
	public GroupsPage(GroupsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}