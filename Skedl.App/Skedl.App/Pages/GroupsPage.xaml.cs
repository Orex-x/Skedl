using Skedl.App.Services;
using Skedl.App.ViewModels;

namespace Skedl.App.Pages;

public partial class GroupsPage : ContentPage
{
    private readonly EventProvider _eventProvider;

    public GroupsPage(GroupsViewModel vm, EventProvider eventProvider)
	{
		InitializeComponent();
        BindingContext = vm;
        _eventProvider = eventProvider;
    }


    

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(100);
        _eventProvider.CallLoadGroups();
    }
}