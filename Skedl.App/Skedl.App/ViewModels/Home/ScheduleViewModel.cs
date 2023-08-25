using CommunityToolkit.Mvvm.ComponentModel;

namespace Skedl.App.ViewModels.Home
{
    public class ScheduleViewModel : ObservableObject
    {
        public ScheduleViewModel() {
            SecureStorage.Default.RemoveAll();
        }
    }
}
