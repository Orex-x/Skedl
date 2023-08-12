namespace Skedl.DataStorage.Models.ApiModels;

public class FieldOfStudy
{
    public string Name { get; set; }
    public ICollection<EduProgramme> EduProgrammes { get; set; }
}