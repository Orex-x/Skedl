using System.Net;
using System.Net.Mail;

namespace Skedl.AuthService.Services.MailService;

public class GmailService : IMailService
{
    private readonly IConfiguration _configuration;
    
    public GmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    } 
    
    public async Task SendMessage(string to, string subject, string message)
    {
        string from = _configuration["GmailSettings:From"]!;
        string pwd = _configuration["GmailSettings:PWD"]!;
        
        var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(from);
        mailMessage.Subject = subject;
        mailMessage.To.Add(new MailAddress(to));
        mailMessage.Body = message;
        mailMessage.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(from, pwd),
            EnableSsl = true,
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}