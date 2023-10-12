using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skedl.AuthService.Models;

public class User
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Login { get; set; }

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } 
    
    public string Password { get; set; }

    public string? AvatarName { get; set; }

    [NotMapped]
    public byte[] Avatar { get; set; }
    
    public int? GroupId { get; set; }
    
    public string? University { get; set; }
    
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
}