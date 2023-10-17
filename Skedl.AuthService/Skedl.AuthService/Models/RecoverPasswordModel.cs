namespace Skedl.AuthService.Models
{
    public class RecoverPasswordModel
    {
        public string OldPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string EmailOrLogin { get; set; } = string.Empty;
    }
}
