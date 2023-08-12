namespace Skedl.DataStorage.Models.ApiModels;

public class EduProgramme
{
    public string Name { get; set; }

    public ICollection<BaseLink> Years { get; set; }
}