using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Skedl.AuthService.Models;

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string? LastName { get; set; }
    
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } 
    
    public string Password { get; set; }
    
    public Group? Group { get; set; }
    
    public string? University { get; set; }
    
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}