using System.ComponentModel.DataAnnotations;

namespace Skedl.AuthService.Models;

public class UserRegister
{
    
    public string Name { get; set; }
    
    public string Login { get; set; }

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } 
    
    public string Password { get; set; }
}