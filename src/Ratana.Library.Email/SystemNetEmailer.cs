using System;
using System.Net;
using System.Net.Mail;

namespace Ratana.Library.Email
{
    /// <summary>
    /// IEmail implementation
    /// </summary>
    public class SystemNetEmailer : IEmailer
    {
        private SmtpClient _smpt { get; set; }

        public SystemNetEmailer(SmtpClient smpt)
        {
            this._smpt = smpt;
        }

        public void Send(MailMessage mailMessage)
        {
            this._smpt.Send(mailMessage);
        }
    }
}
