using System.ComponentModel.DataAnnotations;

namespace Skedl.AuthService.ViewModels;

public class AuthorizationViewModel
{
    [Required(ErrorMessage = "Не указан Email")]
    public string Email { get; set; }
         
    [Required(ErrorMessage = "Не указан пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; } 
}