using System.Net.Mail;

namespace SmartSaver.Domain.Services.EmailServices
{
    public enum MailStatus
    {
        Success,
        Error
    }

    public interface IMailer
    {
        MailStatus SendEmailAsync(MailMessage message);
    }
}
