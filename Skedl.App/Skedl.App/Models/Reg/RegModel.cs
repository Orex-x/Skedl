using Microsoft.AspNetCore.Http;

namespace Skedl.App.Models.Reg
{
    public class RegModel
    {
        public string Email { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public byte[] Avatar { get; set; }
        public string AvatarName { get; set; }
    }
}
