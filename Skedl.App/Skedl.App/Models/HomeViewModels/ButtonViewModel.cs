using CommunityToolkit.Mvvm.ComponentModel;

namespace Skedl.App.Models.HomeViewModels
{
    public partial class ButtonViewModel : ObservableObject
    {
        [ObservableProperty]
        private Brush background;

        public string Text { get; set; }
        public string DayNumber { get; set; }

        public DateTime DateTime { get; set; }
    }
}
