using System.Text.Json.Serialization;

namespace Skedl.AuthService.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Link { get; set; }
}