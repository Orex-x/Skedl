using CommunityToolkit.Mvvm.ComponentModel;
using SpbuParserClient.Models;

namespace SpbuParserClient.ViewModels
{

    [QueryProperty("BaseLink", "Field")]
    public partial class FieldOfStudyViewModel : ObservableObject
    {
        [ObservableProperty]
        BaseLink baseLink;


        public FieldOfStudyViewModel()
        {

        }
    }
}
