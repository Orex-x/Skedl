using System.ComponentModel.DataAnnotations;

namespace Skedl.AuthService.Models;

public class UserRegister
{
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string? LastName { get; set; }
    
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } 
    
    public string Password { get; set; }
}