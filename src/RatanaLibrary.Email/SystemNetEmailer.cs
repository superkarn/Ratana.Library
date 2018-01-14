using System;
using System.Net;
using System.Net.Mail;

namespace RatanaLibrary.Email
{
    /// <summary>
    /// IEmail implementation
    /// </summary>
    public class SystemNetEmailer : IEmailer
    {
        private SmtpClient Smpt { get; set; }

        public SystemNetEmailer(SmtpClient smpt)
        {
            this.Smpt = smpt;
        }

        public void Send(MailMessage mailMessage)
        {
            this.Smpt.Send(mailMessage);
        }
    }
}
