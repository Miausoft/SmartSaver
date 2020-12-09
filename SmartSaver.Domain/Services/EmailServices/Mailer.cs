using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace SmartSaver.Domain.Services.EmailServices
{
    public class Mailer : IMailer
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtp;

        public Mailer(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtp = new SmtpClient(_configuration["Email:Host"], int.Parse(_configuration["Email:Port"]))
            {
                Credentials = new NetworkCredential() { UserName = _configuration["Email:Address"], Password = _configuration["Email:Password"] }
            };
            _smtp.EnableSsl = true;
        }

        public virtual MailStatus SendEmailAsync(MailMessage message)
        {
            try
            {
                _smtp.SendAsync(message, "");
            }
            catch
            {
                return MailStatus.Error;
            }

            return MailStatus.Success;
        }
    }
}
