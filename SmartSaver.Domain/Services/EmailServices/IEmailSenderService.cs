using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SmartSaver.Domain.Services.EmailServices
{
    public enum MailStatus
    {
        Success,
        Error
    }

    public interface IEmailSenderService
    {
        /// <summary>
        /// Sends email with information in Email type object.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        MailStatus SendEmail(MailMessage message);
    }
}
