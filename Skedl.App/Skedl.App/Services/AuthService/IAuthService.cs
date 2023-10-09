using Skedl.App.Models.Api;
using Skedl.App.Models.Reg;

namespace Skedl.App.Services.AuthService
{
    public interface IAuthService
    {
        Task<User> SignInAsync(string emailOrLogin, string password);

        Task<User> RegistrationAsync(RegModel model);

        Task<HttpResponseMessage> SendCodeAsync(string email);
        Task<HttpResponseMessage> SendCodeForRecoverPassword(string emailOrLogin);
        Task<HttpResponseMessage> RecoverPassword(string emailOrLogin, string oldPassword, string newPassword);

        Task<bool> VerifyCodeAsync(string email, string code);

        Task<User> IsAuthorizedAsync();

        Task<string> RefreshTokenAsync(string token);
    }
}
