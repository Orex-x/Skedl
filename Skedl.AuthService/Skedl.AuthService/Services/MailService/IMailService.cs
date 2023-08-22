namespace Skedl.AuthService.Services.MailService;

public interface IMailService
{
    Task SendMessage(string to, string subject, string message);
}