using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Services.EmailServices
{
    public class EmailSenderService : IEmailSenderService
    {
        private const string SmtpServer = "";
        private const string SmtpUsername = "";
        private const string SmtpPassword = "";
        private const int SmtpPort = 465;

        private readonly SmtpClient _client;

        public EmailSenderService()
        {
            _client = new SmtpClient();
        }

        public virtual MailStatus SendEmail(MailMessage message)
        {
            _client.SendAsync(message, "");

            return MailStatus.Success;
        }
    }
}
