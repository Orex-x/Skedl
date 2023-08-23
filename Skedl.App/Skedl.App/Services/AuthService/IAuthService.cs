using Skedl.App.Models.Reg;

namespace Skedl.App.Services.AuthService
{
    public interface IAuthService
    {
        Task Authorization(string emailOrLogin, string password);

        Task<bool> SendCode(string email);

        Task<bool> VerifyCode(string email, string code);

        Task<bool> Registration(RegModel model);
    }
}
